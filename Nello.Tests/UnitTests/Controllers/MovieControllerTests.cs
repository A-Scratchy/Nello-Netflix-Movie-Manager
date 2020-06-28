#region <---------- Using statements ---------->
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Nello.API.Controllers;
using Nello.Data.Interfaces;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using Nello.Data.Models.DBModels;
using Nello.Data.Enums;
using Nello.Data.Domain;
#endregion

namespace Nello.Tests.Controllers
{
    [TestFixture]
    public class MovieControllerTests
    {
        #region ---------- Props ----------
        private MockRepository mockRepository;
        private Mock<ILogger<MovieController>> mockLogger;
        private Mock<IDomainService> mockDomainService;
        private Mock<IDataService> mockDataService;
        #endregion

        #region ---------- SetUp ----------
        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Loose);

            this.mockLogger = this.mockRepository.Create<ILogger<MovieController>>();
            this.mockDomainService = this.mockRepository.Create<IDomainService>();
            this.mockDataService = this.mockRepository.Create<IDataService>();
        }


        private MovieController CreateMovieController()
        {
            return new MovieController(
                this.mockLogger.Object,
                this.mockDomainService.Object,
                this.mockDataService.Object);
        }
        #endregion

        #region <---------- Tests ---------->

        [Test]
        public void GetMovies_GoodQuery_ReturnsListOfMovies()
        {
            // Arrange
            mockDomainService.Setup(d => d.ReadGenresFromString(It.IsAny<string>())).Returns(new List<Genres>() { Genres.Comedy, Genres.Documentary });
            this.mockDomainService.Setup(x => x.CreateMovieViews(It.IsAny<int>(), It.IsAny<FilterModel>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, FilterModel, int, int>((a, x, y, z) => new List<UserMovieModel>()
                {
                    new UserMovieModel( new MovieModel() {
                        Genres = x.Genres.Select(x => x.ToString()).ToList(),
                        Title = x.Keyword,
                        RunTime = x.MaxRuntime - 100,
                        Rating = x.MinRating + 1
                    })
                });
            var movieController = this.CreateMovieController();
            int userId = 1;
            int resultlimit = 100;
            string genres = "[\"Comedy\", \"Documentary\"]";
            string keyword = "Bo";
            int maxruntime = 5000;
            int minrating = 8;

            // Act
            var result = movieController.GetMovies(
                userId,
                resultlimit,
                genres,
                keyword,
                maxruntime,
                minrating);

            // Assert
            Assert.Contains("Comedy", result.First().MovieData.Genres.ToList());
            this.mockRepository.VerifyAll();
        }

        [Test]
        public void GetMoviesInCatalog_HappyPath_ReturnsListOfMovieViews()
        {
            // Arrange
            this.mockDomainService.Setup(x => x.GetMoviesInCatalog(It.IsAny<string>())).Returns(new List<UserMovieModel>()
                {
                new UserMovieModel()
                {
                    MovieData = new MovieModel()
                    {
                        Title = "Big Mommas House", ImdbId = "tt1234567"
                    },
                    UserMoviedata = new UserMoviedataModel()
                }
                }
            );
            var movieController = this.CreateMovieController();
            var catalogId = "123456";

            // Act
            var result = movieController.GetMoviesInCatalog(catalogId);

            // Assert
            Assert.AreEqual(result.First().MovieData.ImdbId, "tt1234567");
            Assert.AreEqual(result.GetType(), typeof(List<UserMovieModel>));
            this.mockRepository.VerifyAll();
        }

        #endregion
    }
}
