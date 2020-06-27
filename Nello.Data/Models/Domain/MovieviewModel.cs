using Nello_Data.Models;
using System;
using System.Collections.Generic;

namespace Nello_Data.Models.Domain
{
    // Converts data into usable view model
    public class MovieviewModel
    {

        public MovieviewModel(MovieModel movie, UserMoviedataModel usermovieData = null)
        {
            this.Movie = movie;

            this.UserMoviedata = usermovieData ?? null;
        }

        public MovieModel Movie { get; set; }

        public UserMoviedataModel UserMoviedata { get; set; }

        public bool UserHasData { get { return (this.UserMoviedata != null); } }
    }
}
