using System;
using System.Collections.Generic;
using System.Linq;
using Nello__Data.Interfaces;
using Nello__Data.Models;
using Nello__Data.Models.Domain;
using Nello__Data.Repos;

namespace Nello_Services
{
    public class DomainService : IDomainService
    {
        private readonly IMongoDBRepo _MongoDBRepo;

        // TODO implement dep inj
        public DomainService(IMongoDBRepo mongoDBRepo)
        {
            _MongoDBRepo = mongoDBRepo;
        }

        public List<MovieviewModel> CreateMovieViews(int userId, FilterModel filters, int resultLimit, int offset)
        {
            var movies = _MongoDBRepo.GetAll<MovieModel>("movies");
            var movieViews = new List<MovieviewModel>();

            if (filters != null)
            {
                movies = movies.Where(m => m.Movie.Rating > filters.MinRating && m.Movie.RunTime < filters.MaxRuntime).ToList();

                if (filters.Keyword != null && filters.Keyword.Length > 0)
                {
                    movies = movies.Where(m => m.Movie.Title.Contains(filters.Keyword)).ToList();
                }
                if (filters.Genres.Count() > 0)
                {
                    movies = movies.Where(m => m.Metadata.Genres.Intersect<string>(filters.Genres.Select(g => g.ToString())).Any()).ToList();
                }
            }

            foreach (var movie in movies.Skip(offset).Take(resultLimit))
            {
                var userdata = _MongoDBRepo.GetById<UserMoviedataModel>("userdata", movie.ImdbId);
                if (userdata != null)
                {
                    var movieview = new MovieviewModel(movie, userdata);
                    movieViews.Add(movieview);
                }
                else
                {
                    var movieview = new MovieviewModel(movie);
                    movieViews.Add(movieview);
                }
            }
            return movieViews;
        }

        public List<CatalogModel> ListCats(int userId)
        {        
            return _MongoDBRepo.GetAll<CatalogModel>("catalogs").ToList();
        }

    }
}
