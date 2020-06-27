using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace Nello_Data.Models

{
    public class MovieModel : TableEntity
    {
        public MovieModel() 
        {
        
        }

        public MovieModel(int year, int id)
        {
            this.PartitionKey = year.ToString();
            this.Year = year;
            this.RowKey = id.ToString();
            this.Id = id;
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("imdbrating")]
        public double Rating { get; set; }

        [JsonProperty("vtype")]
        public string Vtype { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("nfid")]
        public int Nfid { get; set; }

        [JsonProperty("imdbid")]
        public string Imdbid { get; set; }

        [JsonProperty("poster")]
        public string Poster { get; set; }

        [JsonProperty("top250tv")]
        public int Top250Tv { get; set; }

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("avgrating")]
        public double AvgrRting { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("runtime")]
        public int RunTime { get; set; }

        [JsonProperty("top250")]
        public int Top250 { get; set; }
    }
}
