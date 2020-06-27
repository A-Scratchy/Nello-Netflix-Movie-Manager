using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Nello.Data.Models.DBModels

{
    public class UnogsModel
    {
        public UnogsModel()
        {
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public int UnogsId { get; set; }

        [JsonProperty("nfid")]
        public int Nfid { get; set; }

        [BsonId]
        [JsonProperty("imdbid")]
        public string Imdbid { get; set; }

        [JsonProperty("imdbrating")]
        public double Rating { get; set; }

        [JsonProperty("vtype")]
        public string Vtype { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

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
