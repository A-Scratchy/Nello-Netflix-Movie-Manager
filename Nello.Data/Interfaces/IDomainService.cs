using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
using Nello.Data.Models.Enums;
using System.Collections.Generic;

namespace Nello.Data.Interfaces
{
    public interface IDomainService
    {
        List<UserMovieModel> CreateMovieViews(int userId, FilterModel filters, int resultLimit, int offset);
        List<CatalogModel> ListUserCatalogs(int userId);
        List<CatalogModel> ListPublicCatalogs();
        bool ToggleSeen(int userId, string movieId);
        ICollection<Genres> ReadGenresFromString(string genres);
        List<UserMovieModel> GetMoviesInCatalog(string catalogId);
        bool ToggleMovieInCatalog(int userId, string catalogId, string movieId);
        bool MovieIsInAnyUserCatalog(int userId, string movieId);
        bool RenameCatalog(string catalogId, string newName);
        bool MovieIsInCatalog(string catalogId, string movieId);
    }
}