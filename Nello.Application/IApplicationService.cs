using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nello.Application
{
    public interface IApplicationService
    {
        Task<bool> AddCatalog(int userId, string newCategoryName, int newCategoryPrivacyLevel);
        Task<List<UserMovieModel>> GetAndUpdateMovieList(int userId, int resultLimit, string searchTerm = "", int offset = 0, int maxRuntime = 50000, int minRating = 0, List<string> genres = null);
        Task<List<UserMovieModel>> GetMoviesInCatalog(string catalogId);
        Task<List<CatalogModel>> ListPublicCatalogs();
        Task<List<CatalogModel>> ListUserCatalogs(int userId);
        bool MovieIsInAnyUserCatalog(IEnumerable<CatalogModel> catalogs, UserMovieModel movie);
        bool MovieIsInCatalog(CatalogModel catalogId, UserMovieModel movieId);
        Task<bool> RenameCatalog(string catalogId, string newName);
        Task<bool> ToggleMovieInCatalog(int userId, string catalogId, string MovieId);
        Task<bool> ToggleSeen(int userId, string movieId);
        Task<UserMovieModel> GetUpdatedMovie(int userId, UserMovieModel movie);
    }
}