using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nello.Data.Models.DBModels
{
    public class DojoModel
    {

        public DojoModel() { }

        [BsonId]
        public string ImdbId { get { if (Popularity != null) { return Popularity.Id.Substring(7, 9); } else { return null; }; } }

        [JsonProperty("title")]
        public TitleModel Title { get; set; }

        [JsonProperty("ratings")]
        public Ratings Ratings { get; set; }

        [JsonProperty("metacritic")]
        public Metacritic Metacritic { get; set; }

        [JsonProperty("releaseDate")]
        public object ReleaseDate { get; set; }

        [JsonProperty("popularity")]
        public Popularity Popularity { get; set; }

        [JsonProperty("genres")]
        public IList<string> Genres { get; set; }

        [JsonProperty("certificate")]
        public string Certificate { get; set; }
    }

    public class TitleModel
    {

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("runningTimeInMinutes")]
        public int RunningTimeInMinutes { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("titleType")]
        public string TitleType { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }

    public class Ratings
    {

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("titleType")]
        public string TitleType { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("canRate")]
        public bool CanRate { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("ratingCount")]
        public int RatingCount { get; set; }
    }

    public class Metacritic
    {

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("reviewCount")]
        public int ReviewCount { get; set; }

        [JsonProperty("userRatingCount")]
        public int UserRatingCount { get; set; }
    }

    public class Popularity
    {

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("currentRank")]
        public int CurrentRank { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("titleType")]
        public string TitleType { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }

}
