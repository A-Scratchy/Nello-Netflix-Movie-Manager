using System;
using Nello.Data.Models.Domain;
using Nello.Data.Repos;
using Nello.Domain.Services;

namespace Nello_Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var running = true;
            while (running)
            {
                var _MongoDBRepo = new MongoDBRepo();
                var _UnogsRepo = new UnogsRepo();
                var _DojoRepo = new DojoRepo();
                var _DataService = new DataService(_MongoDBRepo, _UnogsRepo, _DojoRepo);
                var _DomainService = new DomainService(_MongoDBRepo, _DataService);

                Console.WriteLine(" --------------- Nello Utils ------------------");
                Console.WriteLine(" 1: Get DB stats");
                Console.WriteLine(" 2: Run DB updates from 3rd party API's");
                Console.WriteLine(" 3: Get MovieViews");
                Console.WriteLine(" 4: Get MovieViews rated 8 and above");
                Console.WriteLine(" 5: Get Comedies");
                Console.WriteLine(" 6: Get Documentaries");
                switch (Console.ReadLine())
                {
                    case "1":
                        _DataService.ShowDbStats();
                        break;
                    case "2":
                        DBUtils(_DataService);
                        break;
                    case "3":
                        foreach (var movieview in _DomainService.CreateMovieViews(1, null, 30, 0))
                        {
                            Console.WriteLine(movieview.MovieData.ImdbId + " " + movieview.MovieData.Title + " : rating " + movieview.MovieData.Rating + " :  has data? " + movieview.UserHasData);
                        }
                        break;
                    case "4":
                        var filter = new FilterModel() { MinRating = 8};
                        foreach (var movieview in _DomainService.CreateMovieViews(1, filter, 30, 0))
                        {
                            Console.WriteLine(movieview.MovieData.ImdbId + " " + movieview.MovieData.Title + " : rating " + movieview.MovieData.Rating + " :  has data? " + movieview.UserHasData);
                        }
                        break;
                    case "5":
                        var Genrefilter = new FilterModel();
                        Genrefilter.Genres.Add(Nello.Data.Models.Enums.Genres.Comedy);
                        foreach (var movieview in _DomainService.CreateMovieViews(1, Genrefilter, 30, 0))
                        {
                            Console.WriteLine(movieview.MovieData.ImdbId + " " + movieview.MovieData.Title + " : rating " + movieview.MovieData.Rating + " :  has data? " + movieview.UserHasData);
                        }
                        break;
                    case "6":
                        var Documentaryfilter = new FilterModel();
                        Documentaryfilter.Genres.Add(Nello.Data.Models.Enums.Genres.Documentary);
                        foreach (var movieview in _DomainService.CreateMovieViews(1, Documentaryfilter, 30, 0))
                        {
                            var genrestring = " | ";
                            foreach (var genre in movieview.MovieData.Genres)
                            {                         
                                genrestring += genre;
                                genrestring += " | ";
                            } 
                            Console.WriteLine(movieview.MovieData.ImdbId + " " + movieview.MovieData.Title + " : rating " + movieview.MovieData.Rating + " :  has data? " + movieview.UserHasData + genrestring);
                        }
                        break;
                    default:
                        break;
                }


            }
        }

        private static void DBUtils(DataService movieService)
        {
            movieService.ShowDbStats();

            Console.WriteLine("Update unogs? Y/N");

            if (Console.ReadLine() == "y")
            {
                // will call unogs API up to 50 times!
                movieService.LoadUnogs(50, 0);
            }

            Console.WriteLine("Are you sure you want to update all records from dojo API? Y/N");

            if (Console.ReadLine() == "y")
            {
                movieService.UpdateMovieRecordsFromUnogs();
                Console.WriteLine("excellent");
            }
            Console.ReadKey();
        }
    }
}
