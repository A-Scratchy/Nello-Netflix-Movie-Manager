#region <---------- Using statements ---------->
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nello.Data.Interfaces;
using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
#endregion
namespace Nello.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        #region <---------- Props ---------->
        private IDomainService _DomainService { get; set; }
        private IDataService _DataService { get; set; }
        #endregion

        #region <---------- Constructor ---------->
        public CatalogController(IDomainService domainService, IDataService dataService)
        {
            _DomainService = domainService;
            _DataService = dataService;
        }
        #endregion

        #region <---------- Endpoints ---------->

        [HttpGet]
        [Route("ListUserCatalogs")]
        public List<CatalogModel> ListUserCatalogs(int userId) =>
            _DomainService.ListUserCatalogs(userId);

        [HttpGet]
        [Route("ListPublicCatalogs")]
        public List<CatalogModel> ListPublicCatalogs() =>
            _DomainService.ListPublicCatalogs();

        [HttpGet]
        [Route("MovieIsInAnyUserCatalog")]
        public bool MovieIsInAnyUserCatalog(int userId, string MovieId) =>
    _DomainService.MovieIsInAnyUserCatalog(userId, MovieId);

        [HttpGet]
        [Route("MovieIsInCatalog")]
        public bool MovieIsInCatalog(string catalogId, string MovieId) =>
    _DomainService.MovieIsInCatalog(catalogId, MovieId);

        [HttpPost]
        [Route("AddCatalog")]
        public bool AddNewCatalog(CatalogModel catalog) =>
           _DataService.AddNewCatalog(catalog);

        [HttpPost]
        [Route("ToggleMovieInCatalog")]
        public bool ToggleMovieInCatalog(int userId, string catalogId, string MovieId) =>
            _DomainService.ToggleMovieInCatalog(userId, catalogId, MovieId);

        [HttpPost]
        [Route("RenameCatalog")]
        public bool RenameCatalog(string catalogId, string newName) =>
            _DomainService.RenameCatalog(catalogId, newName);


        #endregion
    }
}