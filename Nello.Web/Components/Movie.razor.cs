using Microsoft.AspNetCore.Components;
using Nello.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
// this shouldn't be here
using Nello.Data.Models.DBModels;
using System.Diagnostics;
using Nello.Application;

namespace Nello.Web.Components
{
    public class MovieBase : ComponentBase
    {

        [Inject] IApplicationService _ApplicationService { get; set; }

        [Parameter]
        public UserMovieModel Movie { get; set; }

        [Parameter]
        public List<CatalogModel> UsersCatalogs { get; set; }

        [Parameter]
        public EventCallback<UserMovieModel> OnMovieClicked { get; set; }

        private int UserId = 1;
 
        #region <---------- Lifecycle ---------->

        protected override void OnInitialized()
        {
        }

        #endregion

        #region <---------- Callbacks ---------->
        
        public async void SetSelectedMovie()
        {
            // Sends the clicked movie up to parent
            await OnMovieClicked.InvokeAsync(Movie);
        }
        #endregion

        #region <---------- Events ---------->

        public async void ToggleSeen()
        {
            await _ApplicationService.ToggleSeen(UserId, Movie.ToString());
            Movie = await _ApplicationService.GetUpdatedMovie(UserId, Movie);
            StateHasChanged();
        }

        public async void ToggleInCatalog(string catalogId)
        {
            await _ApplicationService.ToggleMovieInCatalog(UserId, catalogId, Movie.ToString());
            Movie = await _ApplicationService.GetUpdatedMovie(UserId, Movie);
            UsersCatalogs = await _ApplicationService.ListUserCatalogs(UserId);
            StateHasChanged();
        }

        #endregion

        #region <---------- Helpers ---------->


        public bool MovieInAnyCatalog()
        {
            return _ApplicationService.MovieIsInAnyUserCatalog(UsersCatalogs, Movie);
        }

        public bool MovieIsInCatalog(CatalogModel catalog)
        {
            return _ApplicationService.MovieIsInCatalog(catalog, Movie);
        }

        #endregion
    }
}
