using Microsoft.AspNetCore.Components;
using Nello.Application;
using Nello.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nello.Web.Components
{
    public class MovieModalBase : ComponentBase
    {

        [Inject] IApplicationService _ApplicationService { get; set; }

        [Parameter]
        public UserMovieModel SelectedMovie { get; set; }

        [Parameter]
        public EventCallback<UserMovieModel> OnMovieUpdated { get; set; }

        private int UserId = 1;


        protected void Hide()
        {
            SelectedMovie = null;
            StateHasChanged();
        }

        protected async void ToggleSeen()
        {
            await _ApplicationService.ToggleSeen(UserId, SelectedMovie.ToString());
            SelectedMovie = await _ApplicationService.GetUpdatedMovie(UserId, SelectedMovie);
            // Let MovieWrapper know movie has been updated
            await OnMovieUpdated.InvokeAsync(SelectedMovie);
        }
    }
}
