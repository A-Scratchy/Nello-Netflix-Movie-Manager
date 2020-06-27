using MongoDB.Bson.Serialization.Attributes;
using Nello.Data.Models.Enums;
using System.Collections.Generic;

namespace Nello.Data.Models.DBModels
{
    public class MovieModel
    {

        [BsonId]
        [BsonRequired]
        public string ImdbId { get; set; }

        public MovieModel() { }

        public MovieModel(UnogsModel unog, DojoModel dojo)
        {
            // from unog
            Title = unog.Title;
            Nfid = unog.Nfid;
            ImdbId = unog.Imdbid;
            Img = unog.Img;
            Synopsis = unog.Synopsis;
            AvgrRting = unog.AvgrRting;
            Year = unog.Year;
            RunTime = unog.RunTime;

            //from dojo
            Rating = dojo.Ratings.Rating;
            RatingCount = dojo.Ratings.RatingCount;
            CurrentRank = dojo.Popularity.CurrentRank;
            ReleaseDate = dojo.ReleaseDate;
            Certificate = dojo.Certificate;
            Genres = dojo.Genres;
        }

        // from unogs

        public string Title { get; set; }

        public int Nfid { get; set; }

        public string Img { get; set; }

        public string Synopsis { get; set; }

        public double AvgrRting { get; set; }

        public int Year { get; set; }

        public int RunTime { get; set; } // in seconds

        // from dojo rating

        public double Rating { get; set; }

        public int RatingCount { get; set; }

        public int CurrentRank { get; set; }

        public object ReleaseDate { get; set; }

        public IList<string> Genres { get; set; }

        public string Certificate { get; set; }

    }
}