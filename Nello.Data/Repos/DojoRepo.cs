using Nello.Data.Models.DBModels;
using Nello.Data.Interfaces;

using Newtonsoft.Json;

using RestSharp;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nello.Data.Repos
{
    public class DojoRepo : IDojoRepo
    {
        readonly string BaseUri = "https://imdb8.p.rapidapi.com/";
        readonly string Key = "cbc01858fdmshc40520b6fbf5788p15dc7cjsn9f4641559692";

        /// <summary> Gets the raw JSON from the web request asking for meta data for the given list of imdb Id's</summary>
        /// <param name="imdbIds">An enumerable of imdb id's</param>
        public string GetMovieMetadataJSON(IEnumerable<string> imdbIds, bool test = false)
        {
            if (test)
            {
                return File.ReadAllText(@"C:\Users\ascratcherd\Desktop\dojo1to50.json");
            }
            else
            {

                var queryString = "title/get-meta-data?region=GB";
                foreach (var imdb in imdbIds)
                {
                    queryString += "&ids=" + imdb;
                }

                var client = new RestClient(BaseUri + queryString);

                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-host", "imdb8.p.rapidapi.com");
                request.AddHeader("x-rapidapi-key", Key);

                IRestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    Debug.WriteLine("successful dojo API call");
                    return response.Content;
                }
                else
                {
                    Debug.WriteLine("failed dojo API call");
                    Debug.WriteLine(response.ErrorMessage);
                    return null;
                }
            }
        }

        /// <summary>Converts string returned from dojoimdb meta endpoint of movie meta-data into metadata data modelk</summary>
        /// <param name="imdbIds">json of movie metadata</param>
        public List<DojoModel> ConvertJsonToModel(string imdbresultstring)
        {
            var json = Regex.Replace(imdbresultstring, "\"tt\\d{7,}\":", "");
            json = json.Replace("{{", "[{");
            json = json.Replace("}}", "}]");
            List<DojoModel> dojos = JsonConvert.DeserializeObject<List<DojoModel>>(json);
            Debug.WriteLine("successfully parsed " + dojos.Count() + " DojoModels");
            return dojos;
        }

        /// <summary> Gets meta data for the given list of imdb Id's</summary>
        /// <param name="imdbIds">An enumerable of imdb id's</param>
        public List<DojoModel> GetMovieMetaData(IEnumerable<string> imdbIds)
        {
            var result = GetMovieMetadataJSON(imdbIds);
            if (result != null)
            {
                return ConvertJsonToModel(result);
            }
            return new List<DojoModel>();
        }
    }
}
