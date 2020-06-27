using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

using System.Collections.Generic;

namespace Nello_Data.Models
{


    public class MovieGenredataModel : TableEntity
    {
        public MovieGenredataModel()
        {

        }

        public MovieGenredataModel(string certificate, string popularity)
        {
            this.PartitionKey = "genre";
            this.RowKey = popularity;
        }

        [JsonProperty("popularity")]
        public Popularity Popularity { get; set; }

        [JsonProperty("genres")]
        public IList<string> Genres { get; set; }

        [JsonProperty("certificate")]
        public string Certificate { get; set; }

        public string ImdbId { get { if (this.Popularity.Id.Length == 17) { return this.Popularity.Id.Substring(7, 9); } else return "???"; } }
    }

    public class Popularity
    {

        [JsonProperty("currentRank")]
        public int CurrentRank { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

    }

}
