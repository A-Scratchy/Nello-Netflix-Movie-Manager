using System.Collections.Generic;
using Nello__Data.Repos;
using Nello__Data.Models;
using Nello__Data.Repositories;
using System.Linq;
using System;
using System.Diagnostics;
using Nello__Data.Interfaces;

namespace Nello_Services
{
    public class DataService : IDataService
    {
        private readonly IMongoDBRepo MongoDBRepo;
        private readonly IDojoRepo DojoRepo;
        private readonly IUnogsRepo UnogsRepo;

        public DataService(IMongoDBRepo mongoDBRepo, IUnogsRepo unogsRepo, IDojoRepo dojoRepo)
        {
            MongoDBRepo = mongoDBRepo;
            UnogsRepo = unogsRepo;
            DojoRepo = dojoRepo;
        }

        #region  -------------------------- Unogs API DB methods ---------------------------------------

        public void SaveUnogsToDB(IEnumerable<UnogsModel> unogs)
        {
            foreach (var unog in unogs)
            {
                if (unog.Imdbid != null)
                {
                    MongoDBRepo.Insert<UnogsModel>("unogs", unog);
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

                var movieJson = UnogsRepo.GetMovieJson(offset);

                if (movieJson != null)
                {
                    var movieResults = UnogsRepo.ConvertJsonToMovieResult(movieJson);

                    // store current value of array
                    var lastCountOfMoviesDB = MongoDBRepo.GetAll<UnogsModel>("unogs").Count();

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
                    $"to {MongoDBRepo.GetAll<UnogsModel>("unogs").Count()} ");
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
            var results = DojoRepo.GetMovieMetaData(imdbIds);
            SaveMetaResultsToMetaCache(results);
        }

        public void SaveMetaResultsToMetaCache(List<DojoModel> metas)
        {
            foreach (var meta in metas)
            {
                if (meta.ImdbId != null)
                {
                    MongoDBRepo.Insert<DojoModel>("dojocache", meta);
                }
            }
        }

        #endregion

        #region -------------------------------- DB stats ---------------------------------------------------

        public void ShowDbStats()
        {
            Console.WriteLine("----------------MONGO DB--------------------");
            Console.WriteLine("unogs records: " + MongoDBRepo.GetAll<UnogsModel>("unogs").Count());
            Console.WriteLine("dojo records: " + MongoDBRepo.GetAll<DojoModel>("dojocache").Count());
            Console.WriteLine("movie records: " + MongoDBRepo.GetAll<MovieModel>("movies").Count());
            Console.WriteLine("------------------------------------------------");
        }

        #endregion

        #region --------------------------- Domain DB methods------------------------------------------------

        public int Create50MovieModelsFromUnogsDBEntries(int offset, bool loadFromAPI)
        {
            var unogs = MongoDBRepo.GetAll<UnogsModel>("unogs").Skip(offset).Take(50);
            var imdbIds = unogs.Select(x => x.Imdbid);
            var movies = new List<MovieModel>();
            var unogsWithNoMeta = new List<string>();
            var totalApiCalls = 0;

            foreach (var unog in unogs)
            {
                // first check metaCahce
                var dojo = MongoDBRepo.GetById<DojoModel>("dojocache", unog.Imdbid);
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

            MongoDBRepo.InsertMany<MovieModel>("movies", movies);
            return totalApiCalls;
        }

        // Main update Method

        public void UpdateMovieRecordsFromUnogs()
        {
            var offset = 0;
            var TotalUnogs = MongoDBRepo.GetAll<UnogsModel>("unogs").Count();
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
                Console.WriteLine("unogs records: " + MongoDBRepo.GetAll<UnogsModel>("unogs").Count());
                Console.WriteLine("dojo records: " + MongoDBRepo.GetAll<DojoModel>("dojocache").Count());
                Console.WriteLine("movie records: " + MongoDBRepo.GetAll<MovieModel>("movies").Count());
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
