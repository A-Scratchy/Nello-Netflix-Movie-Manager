﻿using Microsoft.AspNetCore.Components;
using Nello.Application;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nello.Data.Enums;
using Syncfusion.Blazor.Buttons;

namespace Nello.Web.Pages
{
    public class MoviesBase : ComponentBase
    {
        //TODO change to interface when finished
        [Inject] IApplicationService _ApplicationService { get; set; }

        protected List<UserMovieModel> MovieList;

        protected int UserId;

        protected string SearchTerm = "z";

        protected int resultLimit = 18;

        protected int PageNo = 0;

        protected List<string> filteredGenres = new List<string>();

        protected int maxRuntime = 50000;

        protected int minRating = 0;

        #region <---------- Lifecycle ---------->

        protected async override Task OnInitializedAsync()
        {
            MovieList = await _ApplicationService.GetAndUpdateMovieList(UserId, resultLimit, SearchTerm);
            NextCss = "btn btn-primary";
        }

        #endregion

        #region <---------- Events ---------->

        protected async void Search()
        {
            if (SearchTerm != null)
            {
                PageNo = 0;
                ShowSpinner();
                MovieList = await _ApplicationService.GetAndUpdateMovieList(UserId, resultLimit, SearchTerm, 0, maxRuntime, minRating, filteredGenres);
                if (MovieList.Count < resultLimit)
                {
                    NextCss = "d-none";
                }
                StateHasChanged();
            }
        }

        protected string NextCss = "btn btn-primary";

        protected async void Next()
        {
            PageNo++;
            ShowSpinner();
            var movies = await _ApplicationService.GetAndUpdateMovieList(UserId, resultLimit, SearchTerm, PageNo * resultLimit, maxRuntime, minRating, filteredGenres);
            if (movies.Count > 0)
            {
                MovieList = movies;
                PrevCss = "btn btn-primary";
                if (MovieList.Count < resultLimit)
                {
                    NextCss = "d-none";
                }
            }
            StateHasChanged();
        }

        protected string PrevCss = "d-none";

        protected async void Prev()
        {
            PageNo--;
            ShowSpinner();
            MovieList = await _ApplicationService.GetAndUpdateMovieList(UserId, resultLimit, SearchTerm, PageNo * resultLimit, maxRuntime, minRating, filteredGenres);

            if (PageNo == 0)
            {
                PrevCss = "d-none";
            }
            NextCss = "btn btn-primary";
            StateHasChanged();
        }

        protected void GenreFilterClicked(ClickEventArgs args)
        {
            if (filteredGenres.Contains(args.Text))
            {
                filteredGenres.Remove(args.Text);
            }
            else
            {
                filteredGenres.Add(args.Text);
            }
        }

        #endregion

        #region <---------- Helpers ---------->


        private void ShowSpinner()
        {
            MovieList = null;
            StateHasChanged();
        }

        #endregion
    }

}
