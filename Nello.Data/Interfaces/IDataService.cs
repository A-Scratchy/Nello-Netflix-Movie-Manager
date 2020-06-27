using Nello.Data.Models.DBModels;
using Nello.Data.Models;
using Nello.Data.Models.Domain;
using System.Collections.Generic;

namespace Nello.Data.Interfaces
{
    public interface IDataService
    {
        int Create50MovieModelsFromUnogsDBEntries(int offset, bool loadFromAPI);
        void LoadDojo(IEnumerable<string> imdbIds);
        void LoadUnogs(int callLimit, int startOffset = 0);
        void SaveMetaResultsToMetaCache(List<DojoModel> metas);
        void SaveUnogsToDB(IEnumerable<UnogsModel> unogs);
        void ShowDbStats();
        void UpdateMovieRecordsFromUnogs();
        bool AddNewCatalog(CatalogModel catalog);
        UserMoviedataModel GetMovieData(string movieId, int userId);
        bool UpdateUserMovieData(int userId, UserMoviedataModel userMovieData);
    }
}