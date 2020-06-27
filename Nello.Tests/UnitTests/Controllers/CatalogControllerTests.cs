#region <---------- Using statements ---------->
using Moq;
using NUnit.Framework;
using Nello.API.Controllers;
using Nello.Data.Interfaces;
using Nello.Data.Models.DBModels;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Nello.Tests.Controllers
{
    [TestFixture]
    public class CatalogControllerTests
    {
        #region <---------- Setup ---------->
        private MockRepository mockRepository;

        private Mock<IDomainService> mockDomainService;
        private Mock<IDataService> mockDataService;

        [SetUp]
        public void SetUp()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockDomainService = this.mockRepository.Create<IDomainService>();
            this.mockDataService = this.mockRepository.Create<IDataService>();
        }

        private CatalogController CreateCatalogController()
        {
            return new CatalogController(
                this.mockDomainService.Object,
                this.mockDataService.Object);
        }
        #endregion

        #region <---------- Tests ---------->

        [Test]
        public void ListUserCatalogs_ReturnsUsersCatalogs()
        {
            // Arrange
            mockDomainService.Setup(d => d.ListUserCatalogs(It.IsAny<int>())).
                Returns(new List<CatalogModel>() 
                {
                    new CatalogModel(1, "cat1_user1", "Public"),
                    new CatalogModel(1, "cat2_user1", "Private"),
                });
            var catalogController = this.CreateCatalogController();
            int userId = 1;

            // Act
            var result = catalogController.ListUserCatalogs(userId);

            // Assert
            Assert.AreEqual(2, result.Count());
            this.mockRepository.VerifyAll();
        }

        [Test]
        public void ListPublicCatalogs_ReturnsUsersCatalogs()
        {
            // Arrange
            mockDomainService.Setup(d => d.ListPublicCatalogs()).
                Returns(new List<CatalogModel>() { new CatalogModel(1, "cat1_user1", "Public") });
            var catalogController = this.CreateCatalogController();

            // Act
            var result = catalogController.ListPublicCatalogs();

            // Assert
            Assert.AreEqual(1, result.Count());
            this.mockRepository.VerifyAll();
        }

        [Test]
        public void Post_AddsCatagory()
        {
            // Arrange
            mockDataService.Setup(d => d.AddNewCatalog(It.IsAny<CatalogModel>())).Returns(true);
            var catalogController = this.CreateCatalogController();
            CatalogModel catalog = new CatalogModel(2, "cat4", "Public");

            // Act
            var result = catalogController.AddNewCatalog(catalog);

            // Assert
            Assert.AreEqual(result, catalog);
            this.mockRepository.VerifyAll();
        }

        #endregion
    }
}
