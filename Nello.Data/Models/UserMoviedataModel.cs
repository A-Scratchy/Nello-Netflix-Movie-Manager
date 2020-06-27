using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Models.Domain;

namespace Nello_Data.Models
{
    public class UserMoviedataModel : TableEntity
    {

        public UserMoviedataModel() { }

        public UserMoviedataModel(MovieModel movie, int userId, bool seen)
        {
            Id = Guid.NewGuid();
            MovieId = new Tuple<int, int>(movie.Year, movie.Id);
            UserId = userId;
            PartitionKey = movie.Year.ToString();
            RowKey = Id.ToString();
            Seen = seen;
        }

        public UserMoviedataModel(MovieviewModel movieview, int userId, bool seen)
        {
            Id = Guid.NewGuid();
            MovieId = new Tuple<int, int>(movieview.Movie.Year, movieview.Movie.Id);
            UserId = userId;
            PartitionKey = movieview.Movie.Year.ToString();
            RowKey = movieview.Movie.Id.ToString();
            Seen = seen;
        }

        public Guid Id { get; set; }

        public Tuple<int,int> MovieId { get; set; }

        public int UserId { get; set; }

        public bool Seen { get; set; }

        public int PersonalRating { get; set; }

        public string Notes { get; set; }

    }
}
