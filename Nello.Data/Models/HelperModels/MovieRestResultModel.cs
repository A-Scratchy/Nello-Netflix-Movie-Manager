using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Nello.Data.Models.DBModels;

namespace Nello.Data.Models

{
    public class MovieRestResultModel { 

        public MovieRestResultModel()
        {
        }

        [JsonProperty("elapse")]
        public float Elapsed { get; set; }

        [JsonProperty("results")]
        public ICollection<UnogsModel> Movies { get; set; }

        public DateTime TimeCalled { get; set; }

        public int MoviesFetched { get { if (Movies != null) { return Movies.Count; } else { return 0; } } }
    }

}
