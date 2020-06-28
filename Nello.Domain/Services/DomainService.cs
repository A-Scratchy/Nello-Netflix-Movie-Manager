#region <---------- Using statements ---------->
using MongoDB.Bson;
using MongoDB.Driver;
using Nello.Data.Models.DBModels;
using Nello.Data.Interfaces;
using Nello.Data.Domain;

using System.Collections.Generic;
using System.Linq;
using System;
using Nello.Data.Enums;
using Newtonsoft.Json;
using System.Diagnostics;
using Nello.Data.Models.Domain;
#endregion

namespace Nello.Domain.Services
{
    public class DomainService : IDomainService
    {
        #region <---------- Props ---------->
        private readonly IMongoDBRepo _MongoDBRepo;
        private readonly IDataService _DataService;
        #endregion

        #region <---------- Constructor ---------->
        public DomainService(IMongoDBRepo mongoDBRepo, IDataService IDataService)
        {
            _MongoDBRepo = mongoDBRepo;
            _DataService = IDataService;
        }
        #endregion

        #region <---------- Main methods called from endpoint ---------->

        public List<UserMovieModel> CreateMovieViews(int userId, FilterModel filters, int resultLimit, int offset)
        {
            var ratingFilter = Builders<MovieModel>.Filter.Gt(x => x.Rating, filters.MinRating);
            var runtimeFilter = Builders<MovieModel>.Filter.Lt(x => x.RunTime, filters.MaxRuntime);
            var filter = Builders<MovieModel>.Filter.And(ratingFilter, runtimeFilter);

            if (filters.Keyword != null && filters.Keyword.Length > 0)
            {
                var keyWordFilter = Builders<MovieModel>.Filter.Regex(x => x.Title, new BsonRegularExpression($".*{filters.Keyword}.*", "i"));
                filter = Builders<MovieModel>.Filter.And(filter, keyWordFilter);
            }

            if (filters.Genres.Count() > 0)
            {
                var genreFilter = Builders<MovieModel>.Filter.AnyIn(x => x.Genres, filters.Genres.Select(g => g.ToString()));
                filter = Builders<MovieModel>.Filter.And(filter, genreFilter);
            }
            var movies = _MongoDBRepo.Query("movies", filter);

            var movieViews = new List<UserMovieModel>();

            foreach (var movie in movies.Skip(offset).Take(resultLimit))
            {
                var userdata = _MongoDBRepo.GetById<UserMoviedataModel>("usermoviedata", movie.ImdbId);
                if (userdata != null)
                {
                    var movieview = new UserMovieModel(movie, userdata);
                    movieViews.Add(movieview);
                }
                else
                {
                    var movieview = new UserMovieModel(movie);
                    movieViews.Add(movieview);
                }
            }
            return movieViews;
        }

        public List<UserMovieModel> GetMoviesInCatalog(string catalogId)
        {
            var catalog = _MongoDBRepo.GetById<CatalogModel>("catalog", catalogId);
            var movieViews = new List<UserMovieModel>();
            foreach (var movieId in catalog.Movies)
            {
                var movie = _MongoDBRepo.GetById<MovieModel>("movies", movieId);
                if (movie == null)
                {
                    return null;
                }
                var userdata = _MongoDBRepo.GetById<UserMoviedataModel>("usermoviedata", movieId);
                if (userdata != null)
                {
                    var movieview = new UserMovieModel(movie, userdata);
                    movieViews.Add(movieview);
                }
                else
                {
                    var movieview = new UserMovieModel(movie);
                    movieViews.Add(movieview);
                }
            }
            return movieViews;
        }

        public List<CatalogModel> ListUserCatalogs(int userId)
        {
            var filter = Builders<CatalogModel>.Filter.Eq(x => x.OwnerUserId, userId);
            var result = _MongoDBRepo.Query("catalog", filter).ToList();
            return result;
        }

        public List<CatalogModel> ListPublicCatalogs()
        {
            var filterPublic = Builders<CatalogModel>.Filter.Eq(x => x.PrivacyLevel, PrivacyLevels.Public);
            var filterSystem = Builders<CatalogModel>.Filter.Eq(x => x.PrivacyLevel, PrivacyLevels.System);
            var filter = Builders<CatalogModel>.Filter.Or(filterPublic, filterSystem);
            var result = _MongoDBRepo.Query("catalog", filter).ToList();
            return result;
        }

        public bool RenameCatalog(string catalogId, string newName)
        {
            var catalog = _MongoDBRepo.GetById<CatalogModel>("catalog", catalogId);
            catalog.Name = newName;
            return _MongoDBRepo.Upsert("catalog", catalogId, catalog);
        }

        public bool ToggleSeen(int userId, string movieId)
        {
            var userMovieData = GetOrCreateUserMovieData(userId, movieId);
            userMovieData.Seen = !userMovieData.Seen;
            return _DataService.UpdateUserMovieData(userId, userMovieData);
        }

        public bool ToggleMovieInCatalog(int userId, string catalogId, string movieId)
        {
            //TODO some verifiaction to see if user allowed to add and remove

            var catalog = _MongoDBRepo.GetById<CatalogModel>("catalog", catalogId);

            if (catalog.Movies.Contains(movieId))
            {
                catalog.Movies.Remove(movieId);
            } 
            else
            {
                catalog.Movies.Add(movieId);
            }

            return _MongoDBRepo.Upsert("catalog", catalog.Id, catalog);
        }

        //HACK: This could be a source of poor performance!!!
        public bool MovieIsInAnyUserCatalog(int userId, string movieId)
        {
            var catalogs = ListUserCatalogs(userId);

            foreach (var catalog in catalogs)
            {
                if (catalog.Movies.Contains(movieId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MovieIsInCatalog(string catalogId, string movieId)
        {
            var catalog = _MongoDBRepo.GetById<CatalogModel>("catalog", catalogId);
            return catalog.Movies.Contains(movieId);
        }

        #endregion

        #region <---------- Helpers ---------->

        public ICollection<Genres> ReadGenresFromString(string genres)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Genres>>(genres);
            }
            catch (Exception)
            {
                Debug.WriteLine("cannot parse genre string");
            }
            return null;
        }

        public UserMoviedataModel GetOrCreateUserMovieData(int userId, string movieId) =>
_DataService.GetMovieData(movieId, userId) ?? new UserMoviedataModel(movieId, userId, false);


        #endregion
    }
}
