using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nello.Data.Interfaces;
using Nello.Data.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Nello.Data.Repos
{
    public class UnogsRepo : IUnogsRepo
    {
        readonly string BaseUri = "https://unogsng.p.rapidapi.com/search?";
        readonly string Key = "cbc01858fdmshc40520b6fbf5788p15dc7cjsn9f4641559692";

        public string GetMovieJson(int offset = 0)
        {
            var client = new RestClient(BaseUri +
                "type=movie" +
                "&start_rating=0" +
                "&start_year=1900" +
                "&orderby=rating" +
                "&audiosubtitle_andor=and" +
                "&subtitle=english" +
                "&countrylist=46" +
                "&offset=" + offset.ToString() +
                "&end_year=2020"
                );
            Debug.WriteLine("calling: " + client.BaseUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "unogsng.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", Key);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Debug.WriteLine(response.Content);
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
    }
}
