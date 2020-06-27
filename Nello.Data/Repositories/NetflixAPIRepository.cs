using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nello_Data.Interfaces;
using Nello_Data.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Nello_Data.Repositories
{
    public class NetflixAPIRepository 
    {
        readonly string BaseUri = "https://unogsng.p.rapidapi.com/search?";
        readonly string Key = "cbc01858fdmshc40520b6fbf5788p15dc7cjsn9f4641559692";

        public string GetMovieJson(string limit = "100", string minRating = "0", string type = "movie", string offset = "0")
        {
            var client = new RestClient(BaseUri + 
                "type=" + type +
                "&start_rating=" + minRating +
                "&start_year=1900" +
                "&orderby=rating" +
                "&audiosubtitle_andor=and" +
                "&limit=" + limit +
                "&subtitle=english" +
                "&countrylist=46" +
                "&audio=english" +
                "&offset=" + offset + 
                "&end_year=2020"
                );
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "unogsng.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", Key);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                return null;
            }
        }

        public MovieRestResultModel ConvertJsonToMovieResult(string MovieJson)
        {
            MovieRestResultModel content = JsonConvert.DeserializeObject<MovieRestResultModel>(MovieJson);
            content.TimeCalled = DateTime.Now;
            return content;
        }

        public IEnumerable<MovieModel> GetAllMovies(int callLimit, string limit = "100", string minRating = "0", string type = "movie")
        {
            var movies = new List<MovieModel>();
            var totalResults = 100;
            var totalFetched = 0;
            var calls = 0;

            while (totalFetched < totalResults && calls < callLimit)
            {       
                var offset = totalFetched.ToString();

                var movieJson = GetMovieJson(limit, minRating, type, offset);

                if (movieJson == null)
                {
                    Debug.WriteLine($"call: {calls}, REST call failed, returning {totalFetched} results");
                    return movies;
                }

                var movieResults = ConvertJsonToMovieResult(movieJson);

                // update no of results to fetch
                totalResults = movieResults.TotalNoOfResults;

                // store current value of array
                var lastCountOfMovies = movies.Count();

                // add new results to old results
                movies.AddRange(movieResults.Movies);
                totalFetched = movies.Count();

                // check results have increased count of movies collection
                if (totalFetched <= lastCountOfMovies)
                {
                    throw new Exception();
                }

                Debug.WriteLine($"call:{calls}, fetched {movieResults.MoviesFetched} movies, called with offset {offset}, movie collection changed from {lastCountOfMovies} to {movies.Count()} ");
                calls++;
            }
            return movies;
        }

    }
}
