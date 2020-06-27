using Nello.Data.Models.DBModels;

namespace Nello.Data.Models.Domain
{
    public class UserMovieModel
    {

        public UserMovieModel()
        {

        }

        public UserMovieModel(MovieModel movie, UserMoviedataModel usermovieData = null)
        {
            this.MovieData = movie;
            this.UserMoviedata = usermovieData ?? null;
        }

        public MovieModel MovieData { get; set; }

        public UserMoviedataModel UserMoviedata { get; set; }

        public bool UserHasData { get { return (this.UserMoviedata != null); } }

        public override string ToString()
        {
            return MovieData.ImdbId;
        }
    }
}