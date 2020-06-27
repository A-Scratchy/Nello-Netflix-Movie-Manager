using Nello.Data.Models.DBModels;
using System.Collections.Generic;

namespace Nello.Data.Interfaces
{
    public interface IDojoRepo
    {
        List<DojoModel> ConvertJsonToModel(string imdbresultstring);
        List<DojoModel> GetMovieMetaData(IEnumerable<string> imdbIds);
        string GetMovieMetadataJSON(IEnumerable<string> imdbIds, bool test = false);
    }
}