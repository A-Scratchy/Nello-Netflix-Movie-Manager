using Nello__Data.Models;
using System.Collections.Generic;

namespace Nello_Services
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
    }
}