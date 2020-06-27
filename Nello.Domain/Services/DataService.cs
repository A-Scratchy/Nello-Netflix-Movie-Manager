using System.Collections.Generic;
using Nello.Data.Repos;
using Nello.Data.Models;
using System.Linq;
using System;
using System.Diagnostics;
using Nello.Data.Interfaces;
using Nello.Data.Models.Domain;
using Nello.Data.Models.DBModels;

namespace Nello.Domain.Services
{
    public class DataService : IDataService
    {
        private readonly IMongoDBRepo _MongoDBRepo;
        private readonly IDojoRepo _DojoRepo;
        private readonly IUnogsRepo _UnogsRepo;

        public DataService(IMongoDBRepo mongoDBRepo, IUnogsRepo unogsRepo, IDojoRepo dojoRepo)
        {
            _MongoDBRepo = mongoDBRepo;
            _UnogsRepo = unogsRepo;
            _DojoRepo = dojoRepo;
        }

        #region  -------------------------- Unogs API DB methods ---------------------------------------

        public void SaveUnogsToDB(IEnumerable<UnogsModel> unogs)
        {
            foreach (var unog in unogs)
            {
                if (unog.Imdbid != null)
                {
                    _MongoDBRepo.Insert("unogs", unog);
                }
            }
        }

        public void LoadUnogs(int callLimit, int startOffset = 0)
        {
            var totalNoOfResultsFromQuery = 5000;
            var totalFetched = 0;
            var calls = 0;

            while (totalFetched < totalNoOfResultsFromQuery && calls < callLimit)
            {
                calls++;
                var movies = new List<UnogsModel>();
                var offset = startOffset + totalFetched;

                var movieJson = _UnogsRepo.GetMovieJson(offset);

                if (movieJson != null)
                {
                    var movieResults = _UnogsRepo.ConvertJsonToMovieResult(movieJson);

                    // store current value of array
                    var lastCountOfMoviesDB = _MongoDBRepo.GetAll<UnogsModel>("unogs").Count();

                    // add new results to old results
                    if (movieResults.Movies != null)
                    {
                        movies.AddRange(movieResults.Movies);
                    }

                    totalFetched += movies.Count();

                    Console.WriteLine("saving movies...");
                    SaveUnogsToDB(movies);

                    Console.WriteLine(
                    $"call:{calls}, fetched {movieResults.MoviesFetched} movies," +
                    $" called with offset {offset}, " +
                    $"movie collection changed from {lastCountOfMoviesDB} " +
                    $"to {_MongoDBRepo.GetAll<UnogsModel>("unogs").Count()} ");
                    Console.WriteLine("----");
                }
            }
        }

        #endregion

        #region --------------------------- Dojo API DB methods------------------------------------------------

        public void LoadDojo(IEnumerable<string> imdbIds)
        {
            if (imdbIds.Count() > 50)
            {
                Debug.WriteLine("Cannot call more than 50 in a single request");
                throw new Exception();
            }
            var results = _DojoRepo.GetMovieMetaData(imdbIds);
            SaveMetaResultsToMetaCache(results);
        }

        public void SaveMetaResultsToMetaCache(List<DojoModel> metas)
        {
            foreach (var meta in metas)
            {
                if (meta.ImdbId != null)
                {
                    _MongoDBRepo.Insert("dojocache", meta);
                }
            }
        }

        #endregion

        #region -------------------------------- DB stats ---------------------------------------------------

        public void ShowDbStats()
        {
            Console.WriteLine("----------------MONGO DB--------------------");
            Console.WriteLine("unogs records: " + _MongoDBRepo.GetAll<UnogsModel>("unogs").Count());
            Console.WriteLine("dojo records: " + _MongoDBRepo.GetAll<DojoModel>("dojocache").Count());
            Console.WriteLine("movie records: " + _MongoDBRepo.GetAll<MovieModel>("movies").Count());
            Console.WriteLine("------------------------------------------------");
        }

        #endregion

        #region ------------------------Catalog DB methods -------------------------------------

        public bool AddNewCatalog(CatalogModel catalog)
        {
            return _MongoDBRepo.Insert("catalog", catalog);
        }

        #endregion

        #region ------------------------UserData DB methods -------------------------------------

        public UserMoviedataModel GetMovieData(string movieId, int userId) =>
            _MongoDBRepo.GetById<UserMoviedataModel>("usermoviedata", movieId);

        public bool UpdateUserMovieData(int userId, UserMoviedataModel userMovieData)
        {
            var result = _MongoDBRepo.Upsert("usermoviedata", userMovieData.Id, userMovieData);
            return result;
        }

        #endregion

        #region --------------------------- DB Utils methods------------------------------------------------

        public int Create50MovieModelsFromUnogsDBEntries(int offset, bool loadFromAPI)
        {
            var unogs = _MongoDBRepo.GetAll<UnogsModel>("unogs").Skip(offset).Take(50);
            var imdbIds = unogs.Select(x => x.Imdbid);
            var movies = new List<MovieModel>();
            var unogsWithNoMeta = new List<string>();
            var totalApiCalls = 0;

            foreach (var unog in unogs)
            {
                // first check metaCahce
                var dojo = _MongoDBRepo.GetById<DojoModel>("dojocache", unog.Imdbid);
                if (dojo != null)
                {
                    var movie = new MovieModel(unog, dojo);
                    movies.Add(movie);
                }
                else
                // Add to a list to fetch from dojo api
                {
                    unogsWithNoMeta.Add(unog.Imdbid);
                }
            }
            // If it's an new batch of movies with no meta, get from the dojo api
            if (unogsWithNoMeta.Count() > 49 && loadFromAPI)
            {
                LoadDojo(unogsWithNoMeta);
                totalApiCalls++;
            }
            else
            {
                Debug.Write("did not fetch data for following id's : ");
                foreach (var id in unogsWithNoMeta) { Debug.Write(id + ", "); }
                Debug.WriteLine(" | Total: " + unogsWithNoMeta.Count());
            }

            _MongoDBRepo.InsertMany("movies", movies);
            return totalApiCalls;
        }

        // Main update Method

        public void UpdateMovieRecordsFromUnogs()
        {
            var offset = 0;
            var TotalUnogs = _MongoDBRepo.GetAll<UnogsModel>("unogs").Count();
            var requiredCalls = TotalUnogs / 50;
            var totalApiCalls = 0;
            Console.WriteLine("Total unogs = " + TotalUnogs);
            Console.WriteLine("Total potential api calls = " + requiredCalls);
            Console.WriteLine("-----------------POPULATING MOVIE DB------------------------");
            for (int i = 1; i <= requiredCalls; i++)
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("call " + i + "/" + requiredCalls + " | " + "offset = " + offset);
                totalApiCalls += Create50MovieModelsFromUnogsDBEntries(offset, true);
                Console.WriteLine("called DOJO API " + totalApiCalls + " times");
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("unogs records: " + _MongoDBRepo.GetAll<UnogsModel>("unogs").Count());
                Console.WriteLine("dojo records: " + _MongoDBRepo.GetAll<DojoModel>("dojocache").Count());
                Console.WriteLine("movie records: " + _MongoDBRepo.GetAll<MovieModel>("movies").Count());
                Console.WriteLine("--------------------------------------------------------");
                offset += 50;
            }
            if (totalApiCalls > 0)
            {
                Console.WriteLine("Updating movie DB with new dojometas");
                for (int i = 1; i <= requiredCalls; i++)
                {
                    Create50MovieModelsFromUnogsDBEntries(0, false);
                    offset += 50;
                }
            }
            ShowDbStats();
            Console.WriteLine("-----------------FINISHED------------------------");
        }

        #endregion
    }
}
