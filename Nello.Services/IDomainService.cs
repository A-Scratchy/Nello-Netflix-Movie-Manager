using Nello__Data.Models;
using Nello__Data.Models.Domain;
using System.Collections.Generic;

namespace Nello_Services
{
    public interface IDomainService
    {
        List<MovieviewModel> CreateMovieViews(int userId, FilterModel filters, int resultLimit, int offset);
        List<CatalogModel> ListCats(int userId);
    }
}