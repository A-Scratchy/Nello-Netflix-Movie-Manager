using System.Collections.Generic;
using Nello.Data.Models.DBModels;

namespace Nello.Data.Models.Domain
{
    // Converts data into usable view model
    public class CatalogViewModel
    {

        public CatalogViewModel(CatalogModel catalog, IEnumerable<UserMovieModel> movies)
        {
            Movies = movies;
            Catalog = catalog;
        }

        public CatalogModel Catalog { get; set; }

        public IEnumerable<UserMovieModel> Movies { get; set; }
    }
}
