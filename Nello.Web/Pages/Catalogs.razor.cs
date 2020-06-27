using Microsoft.AspNetCore.Components;

using Nello.Application;
using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nello.Web.Pages
{
    public class CatalogsBase : ComponentBase
    {
        [Inject] IApplicationService _ApplicationService { get; set; }

        protected List<CatalogModel> CatalogList;

        private CatalogModel SelectedCatalog;

        protected string NewCategoryName;

        private int NewCategoryPrivacyLevel = 1;

        private int UserId = 1;

        protected List<UserMovieModel> CatalogMovies;

        protected async override Task OnInitializedAsync()
        {
            CatalogList = await _ApplicationService.ListUserCatalogs(UserId);
        }

        protected async void OnCatalogSelect(CatalogModel catalog)
        {
            SelectedCatalog = catalog;
            CatalogMovies = await _ApplicationService.GetMoviesInCatalog(catalog.Id);
            StateHasChanged();
        }

        protected async void CreateNewCatalog()
        {
            await _ApplicationService.AddCatalog(UserId, NewCategoryName, NewCategoryPrivacyLevel);
            StateHasChanged();
        }
    }
}
