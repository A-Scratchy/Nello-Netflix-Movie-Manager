#region <---------- Using statements ---------->
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Nello.Data.Interfaces;
using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;

using System.Collections.Generic;
#endregion

namespace Nello.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        #region <---------- Props ---------->
        private readonly ILogger<MovieController> _logger;
        private IDomainService _DomainService { get; set; }
        private IDataService _DataService { get; set; }
        #endregion

        #region <---------- Constructor ---------->
        public MovieController(ILogger<MovieController> logger, IDomainService domainService, IDataService dataService)
        {
            _logger = logger;
            _DomainService = domainService;
            _DataService = dataService;
        }
        #endregion

        #region <---------- Endpoints ---------->

        [HttpGet]
        [Route("GetMovies")]
        public List<UserMovieModel> GetMovies(int userId, int resultlimit, string genres, string keyword, int offset = 0, int maxruntime = 10000, double minrating = 0)
        {
            var filters = new FilterModel { MinRating = minrating, MaxRuntime = maxruntime, Keyword = keyword };
            filters.Genres = _DomainService.ReadGenresFromString(genres);

            return _DomainService.CreateMovieViews(userId, filters, resultlimit, offset);
        }

        [HttpGet]
        [Route("GetMoviesInCatalog")]
        public List<UserMovieModel> GetMoviesInCatalog(string catalogId) =>
            _DomainService.GetMoviesInCatalog(catalogId);

        //TODO: Use imdb api to provide reviews etc... on demand
        [HttpGet]
        [Route("GetMoreMovieDetails")]
        public List<UserMovieModel> GetMoreMovieDetails(string catalogId) =>
        null;

        #endregion
    }
}
