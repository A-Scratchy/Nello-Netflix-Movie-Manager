using Nello.Data.Models.DBModels;
using Nello.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Nello.Application
{
    /// <summary>
    /// This class calls the Nello API service and returns the results to the UI layer.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        IHttpClientFactory _ClientFactory;

        public ApplicationService(IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
        }

        //TODO get base uri from config or set in startup??
        private readonly string BaseUri = "https://localhost:44326/api/";

        #region <---------- Catalog Endpoint ---------->

        // ​/api​/Catalog​/ListUserCatalogs GET
        public async Task<List<CatalogModel>> ListUserCatalogs(int userId)
        {
            var requestString = BaseUri + $"Catalog/ListUserCatalogs?UserId={userId}";
            var client = _ClientFactory.CreateClient();

            try
            {
                return await client.GetFromJsonAsync<List<CatalogModel>>(requestString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error getting catalogs: " + e);
            }
            return new List<CatalogModel>();
        }

        // /api/Catalog/ListPublicCatalogs GET
        public async Task<List<CatalogModel>> ListPublicCatalogs()
        {
            var requestString = BaseUri + "Catalog/ListPublicCatalogs";

            var client = _ClientFactory.CreateClient();
            try
            {
                return await client.GetFromJsonAsync<List<CatalogModel>>(requestString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return new List<CatalogModel>();
        }

        // /api​/Catalog​/MovieIsInAnyUserCatalog GET
        public bool MovieIsInAnyUserCatalog(IEnumerable<CatalogModel> catalogs, UserMovieModel movie)
        {
            //var requestString = BaseUri + $"Catalog/MovieIsInAnyUserCatalog?userId={userId}&MovieId={MovieId}";

            //var client = _ClientFactory.CreateClient();
            //try
            //{
            //    var response = await client.GetStringAsync(requestString).ConfigureAwait(false);
            //    Debug.WriteLine(response);
            //    return bool.Parse(response);
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e);
            //}
            if (catalogs != null)
            {
                foreach (var catalog in catalogs)
                {
                    if (MovieIsInCatalog(catalog, movie))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // /api/Catalog/MovieIsInCatalog GET
        public bool MovieIsInCatalog(CatalogModel catalog, UserMovieModel movie)
        {
            //var requestString = BaseUri + $"Catalog/MovieIsInCatalog?catalogId={catalogId}&movieId={movieId}";

            //var client = _ClientFactory.CreateClient();
            //try
            //{
            //    var response = await client.GetStringAsync(requestString).ConfigureAwait(false);
            //    return bool.Parse(response);
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e);
            //}
            var result = catalog.Movies.Contains(movie.MovieData.ImdbId);
            return result;
        }

        //TODO /api/Catalog/RenameCatalog POST
        public async Task<bool> RenameCatalog(string catalogId, string newName)
        {
            var requestString = BaseUri + $"Catalog/RenameCatalog?catalogId={catalogId}&newName={newName}";

            var client = _ClientFactory.CreateClient();
            try
            {
                var response = await client.PostAsync(requestString, null);
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                Debug.WriteLine("RenameCatalog returned: " + result);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }

        // ​/api​/Catalog​/AddCatalog POST
        public async Task<bool> AddCatalog(int userId, string newCategoryName, int newCategoryPrivacyLevel)
        {
            if (!String.IsNullOrEmpty(newCategoryName))
            {
                var requestString = BaseUri + $"Catalog/AddCatalog?id={userId}";
                var client = _ClientFactory.CreateClient();
                var body = "{\"ownerUserId\":" + userId + ",\"name\":\"" + newCategoryName 
                    + "\",\"privacyLevel\": " + newCategoryPrivacyLevel + "}";
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(requestString, content);
                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    Debug.WriteLine("AddCatalog returned: " + result);
                    return result;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return false;
                }
            }
            return false;
        }

        // /api/Catalog/ToggleMovieInCatalog POST
        public async Task<bool> ToggleMovieInCatalog(int userId, string catalogId, string MovieId)
        {
            var requestString = BaseUri + $"Catalog/ToggleMovieInCatalog?userId={userId}&catalogId={catalogId}&MovieId={MovieId}";
            var client = _ClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(requestString, null);
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                Debug.WriteLine("ToggleMovieInCatalog returned: " + result);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return false;
        }

        #endregion

        #region <---------- Movie Endpoint ---------->

        // ​/api​/Movie​/GetMovies GET
        public async Task<List<UserMovieModel>> GetAndUpdateMovieList(int userId, int resultLimit, string searchTerm = "", int offset = 0,
                             int maxRuntime = 50000, int minRating = 0, List<string> genres = null)
        {
            var requestString = BaseUri + "Movie/GetMovies?";
            requestString += $"userId={userId}";
            requestString += $"&resultlimit={resultLimit}";
            requestString += $"&offset={offset}";
            requestString += $"&maxruntime={maxRuntime}";
            requestString += $"&minRating={minRating}";
            requestString += "&genres=[";
            if(genres != null)
            {
                genres.Select(g => requestString += g);
            }
            requestString += "]";
            requestString += $"&keyword={searchTerm}";

            var client = _ClientFactory.CreateClient();

            try
            {
                return await client.GetFromJsonAsync<List<UserMovieModel>>(requestString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error calling Movie Endpoint: " + e);
            }
            return new List<UserMovieModel>();
        }

        // /api/Movie/GetMoreMovieDetails

        // /api/Movie/GetMoviesInCatalog GET
        public async Task<List<UserMovieModel>> GetMoviesInCatalog(string catalogId)
        {
            var requestString = BaseUri + $"Movie/GetMoviesInCatalog?catalogId={catalogId}";

            var client = _ClientFactory.CreateClient();
            try
            {
                return await client.GetFromJsonAsync<List<UserMovieModel>>(requestString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return new List<UserMovieModel>();
        }

        // TODO: /api/Movie/GetMovie
        public async Task<UserMovieModel> GetUpdatedMovie(int userId, UserMovieModel movie)
        {
            //TODO create an actual get movie endpoint
            return (await GetAndUpdateMovieList(userId, 1, movie.MovieData.Title)).First();
        }


        #endregion

        #region <---------- UserData Endpoint ---------->

        // /api/Userdata/ToggleSeen GET
        public async Task<bool> ToggleSeen(int userId, string movieId)
        {
            var requestString = BaseUri + $"Userdata/ToggleSeen?userId={userId}&movieId={movieId}";

            var client = _ClientFactory.CreateClient();
            try
            {
                return await client.GetFromJsonAsync<bool>(requestString);
            }
            catch (Exception e)
            {
                Debug.WriteLine("There was a problem calling ToggleSeen endpoint: " +  e);
            }
            return false;
        }

        #endregion

    }
}
