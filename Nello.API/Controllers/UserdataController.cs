#region <---------- Using statements ---------->
using Microsoft.AspNetCore.Mvc;

using Nello.Data.Interfaces;
using Nello.Data.Models.DBModels;
#endregion

namespace Nello.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserdataController : ControllerBase
    {
        #region <---------- Props ---------->
        private IDomainService _DomainService { get; set; }
        private IDataService _DataService { get; set; }
        #endregion

        #region <---------- Constructor ---------->
        public UserdataController(IDomainService domainService, IDataService dataService)
        {
            _DomainService = domainService;
            _DataService = dataService;
        }
        #endregion

        #region <---------- Endpoints ---------->

        /// <summary>
        /// Toggles the userdata to indicate wether or not the user has seen the movie
        /// </summary>
        /// <param name="userId">A user Id</param>
        /// <param name="movieId">The imdb id of the movie e.g "tt1234567"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ToggleSeen")]
        public bool ToggleSeen(int userId, string movieId) =>
            _DomainService.ToggleSeen(userId, movieId);

        #endregion
    }
}