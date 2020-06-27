using Microsoft.AspNetCore.Components;
using Nello.Application;
using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nello.Web.Components
{
    public class MovieWrapperComponentBase : ComponentBase
    {
        [Inject] IApplicationService _ApplicationService { get; set; }

        [Parameter]
        public List<UserMovieModel> Movies { get; set; }

        protected List<CatalogModel> UserCatalogs;

        protected UserMovieModel SelectedMovie = null;

        protected int userId = 1;

        #region <---------- Lifecycle ---------->

        protected async override Task OnInitializedAsync()
        {
            UserCatalogs = await _ApplicationService.ListUserCatalogs(userId);
        }

        #endregion

        #region <---------- Events ---------->

        public void HandleMovieClicked(UserMovieModel movie)
        {
            var i = Movies.FindIndex(m => m.MovieData.ImdbId == movie.MovieData.ImdbId);
            SelectedMovie = Movies[i];
        }
        

        public void HandleMovieUpdatedFromModal(UserMovieModel movie)
        {
            var i = Movies.FindIndex(m => m.MovieData.ImdbId == movie.MovieData.ImdbId);
            Movies[i] = movie;
            SelectedMovie = movie;
            StateHasChanged();
        }

        #endregion

    }
}
