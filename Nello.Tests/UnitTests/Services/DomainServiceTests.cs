#region <---------- Using Statements ---------->
using Moq;
using NUnit.Framework;
using Nello.Data.Interfaces;
using Nello.Domain.Services;
using Nello.Data.Models.Domain;
using System.Collections.Generic;
using Nello.Data.Models.DBModels;
using System.Linq;
using MongoDB.Driver;
using Nello.Data.Enums;
using Nello.Data.Domain;
#endregion
namespace Nello.Tests.Services
{
    [TestFixture]
    public class DomainServiceTests
    {
        #region <---------- SetUp ---------->

        private MockRepository mockRepository;

        private Mock<IMongoDBRepo> mockMongoDBRepo;
        private Mock<IDataService> mockDataService;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockMongoDBRepo = mockRepository.Create<IMongoDBRepo>();
            mockDataService = mockRepository.Create<IDataService>();
        }

        private DomainService CreateService() =>
            new DomainService(mockMongoDBRepo.Object, mockDataService.Object);

        #endregion

        #region <---------- Tests ---------->

        [Test]
        public void CreateMovieViews_HappyPath_ReturnsList()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.Query<MovieModel>("movies", It.IsAny<FilterDefinition<MovieModel>>())).
                Returns(new List<MovieModel>()
                {
                 new MovieModel() { Title = "Big Mommas House", ImdbId = "tt1234567" },
                 new MovieModel() { Title = "Big Mommas House 2", ImdbId = "tt1234568" }
                }
            );
            mockMongoDBRepo.Setup(m => m.GetById<UserMoviedataModel>("usermoviedata", It.IsAny<string>())).
                Returns(new UserMoviedataModel() { Id = "tt1234567", UserId = 1, Seen = true });
            mockMongoDBRepo.Setup(m => m.GetById<UserMoviedataModel>("usermoviedata", It.IsAny<string>())).
                Returns(() => null);

            // Act
            var result = service.CreateMovieViews(1, new FilterModel(), 2, 0);

            // Assert
            Assert.AreEqual(result.Count, 2);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetMoviesInCatalog_HappyPath_ReturnMovieViewmodel()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.GetById<MovieModel>("movies", "tt0032138")).
                Returns(new MovieModel() { ImdbId = "tt0032138", Title = "Big mommas house" });
            mockMongoDBRepo.Setup(m => m.GetById<MovieModel>("movies", "tt0034498")).
                Returns(new MovieModel() { ImdbId = "tt0034498", Title = "Big mommas house 2" });
            mockMongoDBRepo.Setup(m => m.GetById<UserMoviedataModel>("usermoviedata", "tt0032138")).
                Returns(() => null);
            mockMongoDBRepo.Setup(m => m.GetById<UserMoviedataModel>("usermoviedata", "tt0034498")).
                Returns(new UserMoviedataModel() { Id = "tt0034498", UserId = 1 });
            mockMongoDBRepo.Setup(m => m.GetById<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78")).
                Returns(new CatalogModel(1, "testCat", "public") { Movies = new List<string>() { "tt0032138", "tt0034498" } });

            // Act
            var result = service.GetMoviesInCatalog("07ac821b-e795-4604-a029-3771fec0ca78");

            // Assert
            Assert.AreEqual(result.GetType(), typeof(List<UserMovieModel>));
            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result.First().MovieData.Title, "Big mommas house");
            Assert.AreEqual(result.First().UserMoviedata, null);
            Assert.AreEqual(result.Skip(1).First().UserMoviedata.UserId, 1);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetMoviesInCatalog_BadStrings_FailSilent()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.GetById<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78")).
                Returns(new CatalogModel(1, "testCat", "public"));

            // Act
            var result = service.GetMoviesInCatalog("07ac821b-e795-4604-a029-3771fec0ca78");

            // Assert
            Assert.AreEqual(result.Count(), 0);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ListUserCatalogs_HappyPath_ReturnsList()
        {
            // Arrange
            var service = CreateService();
            int userId = 1;
            mockMongoDBRepo.Setup(m => m.Query("catalog", It.IsAny<FilterDefinition<CatalogModel>>())).
                Returns(new List<CatalogModel>() { new CatalogModel(1, "testCat", "public") });

            // Act
            var result = service.ListUserCatalogs(userId);

            // Assert
            Assert.AreEqual(result.Count(), 1);
            Assert.AreEqual(result.GetType(), typeof(List<CatalogModel>));
            mockRepository.VerifyAll();
        }

        [Test]
        public void ListAllPublicCatalogs_HappyPath_ReturnsList()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.Query("catalog", It.IsAny<FilterDefinition<CatalogModel>>())).
                Returns(new List<CatalogModel>() { new CatalogModel(1, "testCat", "public") });

            // Act
            var result = service.ListPublicCatalogs();

            // Assert
            Assert.AreEqual(result.Count(), 1);
            Assert.AreEqual(result.GetType(), typeof(List<CatalogModel>));
            mockRepository.VerifyAll();
        }

        [Test]
        public void ToggleSeen_HappyPath_returnsTrue()
        {
            // Arrange
            var service = CreateService();
            mockDataService.Setup(d => d.UpdateUserMovieData(It.IsAny<int>(), It.IsAny<UserMoviedataModel>())).
                Returns(true); // Indicates data saved ok
            mockDataService.Setup(d => d.GetMovieData(It.IsAny<string>(), It.IsAny<int>())).
                Returns(new UserMoviedataModel("tt1234567", 1, false)); // Retuned movie with seen as false

            // Act
            var result = service.ToggleSeen(1, "tt1234567");

            // Assert
            Assert.IsTrue(result);
            mockRepository.VerifyAll();

        }

        [Test]
        public void ToggleSeen_DbWriteFailed_returnsFalse()
        {
            // Arrange
            var service = CreateService();
            mockDataService.Setup(d => d.UpdateUserMovieData(It.IsAny<int>(), It.IsAny<UserMoviedataModel>())).
                Returns(false); // Indicates data did not write
            mockDataService.Setup(d => d.GetMovieData(It.IsAny<string>(), It.IsAny<int>())).
                Returns(new UserMoviedataModel("tt1234567", 1, false)); // Retuned movie with seen as false

            // Act
            var result = service.ToggleSeen(1, "tt1234567");

            // Assert
            Assert.IsFalse(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ReadGenresFromString_HappyPath_returnsListOfGenres()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.ReadGenresFromString("[\"Comedy\",\"Documentary\"]");

            // Assert
            Assert.AreEqual(result.First(), Genres.Comedy);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ReadGenresFromString_BadString_returnsNull()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.ReadGenresFromString("(Comedy,Documentary)");

            // Assert
            Assert.AreEqual(result, null);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ListCatalogs_HappyPath_returnsListOfCatalogs()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.Query("catalog", It.IsAny<FilterDefinition<CatalogModel>>())).
                Returns(new List<CatalogModel>()
                {
                 new CatalogModel(1,"testCat1", "public"),
                 new CatalogModel(1,"testCat2", "private")
                }
            );

            // Act
            var result = service.ListUserCatalogs(1);

            // Assert
            Assert.AreEqual(result.First().Name, "testCat1");
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetOrCreateUserMovieData_HappyPath_returnsUserData()
        {
            // Arrange
            var service = CreateService();
            mockDataService.Setup(d => d.GetMovieData("tt1234567", It.IsAny<int>())).
                Returns(new UserMoviedataModel() { Id = "tt1234567", UserId = 1 });

            // Act
            var result = service.GetOrCreateUserMovieData(1, "tt1234567");

            // Assert
            Assert.AreEqual(result.Id, "tt1234567");
            mockRepository.VerifyAll();
        }

        [Test]
        public void ToggleMovieInCatalog_RemoveMovie_returnsTrue()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.GetById<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78")).
                Returns(new CatalogModel(1, "testCat", "public") { Movies = new List<string>() { "tt1234567" } });
            mockMongoDBRepo.Setup(d => d.Upsert("catalog", It.IsAny<string>(), It.IsAny<CatalogModel>())).
                Returns(true);

            // Act
            var result = service.ToggleMovieInCatalog(1, "07ac821b-e795-4604-a029-3771fec0ca78", "tt1234567");

            // Assert
            Assert.IsTrue(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ToggleMovieInCatalog_AddMovie_returnsTrue()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.GetById<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78")).
                Returns(new CatalogModel(1, "testCat", "public") { Movies = new List<string>() });
            mockMongoDBRepo.Setup(d => d.Upsert("catalog", It.IsAny<string>(), It.IsAny<CatalogModel>())).
                Returns(true);

            // Act
            var result = service.ToggleMovieInCatalog(1, "07ac821b-e795-4604-a029-3771fec0ca78", "tt1234567");

            // Assert
            Assert.IsTrue(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void MovieIsInAnyUserCatalog_HappyPathTrue_returnsTrue()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.Query("catalog", It.IsAny<FilterDefinition<CatalogModel>>())).
                Returns(new List<CatalogModel>()
                {
                    new CatalogModel(1, "testCat1", "public") { Movies = new List<string>()},
                    new CatalogModel(1, "testCat2", "public") { Movies = new List<string>() { "tt1234567" }}
                });

            // Act
            var result = service.MovieIsInAnyUserCatalog(1, "tt1234567");

            // Assert
            Assert.IsTrue(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void MovieIsInAnyUserCatalog_HappyPathFalse_returnsFalse()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.Query("catalog", It.IsAny<FilterDefinition<CatalogModel>>())).
                Returns(new List<CatalogModel>()
                {
                    new CatalogModel(1, "testCat1", "public") { Movies = new List<string>()},
                    new CatalogModel(1, "testCat2", "public") { Movies = new List<string>() { "tt2345678" }}
                });

            // Act
            var result = service.MovieIsInAnyUserCatalog(1, "tt1234567");

            // Assert
            Assert.IsFalse(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RenameCatalog_HappyPath_returnsTrue()
        {
            // Arrange
            var service = CreateService();
            mockMongoDBRepo.Setup(m => m.GetById<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78")).
                Returns( new CatalogModel(1, "TestCat1", "private") 
            { Id = "07ac821b-e795-4604-a029-3771fec0ca78" });
            mockMongoDBRepo.Setup(m => m.Upsert<CatalogModel>("catalog", "07ac821b-e795-4604-a029-3771fec0ca78", It.IsAny<CatalogModel>())).
                Returns(true);

            // Act
            var result = service.RenameCatalog("07ac821b-e795-4604-a029-3771fec0ca78", "TestCat2");

            // Assert
            Assert.IsTrue(result);
            mockRepository.VerifyAll();
        }

        #endregion
    }
}
