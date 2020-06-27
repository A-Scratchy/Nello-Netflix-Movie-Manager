using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nello_Data.Models

{
    public class MovieRestResultModel
    {
        [JsonProperty("elapse")]
        public float Elapsed { get; set; }

        [JsonProperty("results")]
        public ICollection<MovieModel> Movies { get; set; }

        [JsonProperty("total")]
        public int TotalNoOfResults { get; set; }

        public DateTime TimeCalled { get; set; }

        public int MoviesFetched { get { return this.Movies.Count; } }
    }

}
