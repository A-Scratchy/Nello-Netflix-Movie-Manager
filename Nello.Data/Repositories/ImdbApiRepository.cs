using Nello_Data.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;


namespace Nello_Data.Repositories
{
    public class ImdbApiRepository
    {
        readonly string BaseUri = "https://imdb8.p.rapidapi.com/";
        readonly string Key = "cbc01858fdmshc40520b6fbf5788p15dc7cjsn9f4641559692";

        /// <summary> Gets the raw JSON from the web request asking fro meta data for the given list of imdb Id's</summary>
        /// <param name="imdbIds">An enumerable of imdb id's</param>
        public string GetMovieMetadataJSON(IEnumerable<string> imdbIds)
        {
            var queryString = "title/get-meta-data?region=GB";
            foreach (var imdb in imdbIds)
            {
                queryString += "&ids=" + imdb;
            }
            
            Debug.WriteLine(queryString);

            var client = new RestClient(BaseUri + queryString);

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

        /// <summary>Converts raw JSON of movie meta-data into metadata data modelk</summary>
        /// <param name="imdbIds">json of movie metadata</param>
        public List<MovieGenredataModel> ConvertJsonToModel(string json)
        {
            List<MovieGenredataModel> content = JsonConvert.DeserializeObject<List<MovieGenredataModel>>(json);
            return content;
        }
    }
}
