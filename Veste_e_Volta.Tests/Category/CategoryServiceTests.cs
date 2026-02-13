
using Moq;
using NUnit.Framework;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;
using System.Collections.Generic;

namespace Veste_e_Volta.Tests.Category
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private  Mock<ICategoryRepositories> _categoryRepositoryMock;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepositories>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object);
        }

        [Test]
        public void GetCategories_WhenThereAreNoCategories_ShouldReturnEmptyList()
        {
            // Arrange
            _categoryRepositoryMock
                .Setup(repo => repo.GetAll())
                .Returns(new List<TbCategory>());

            // Act
            var result = _categoryService.GetCategories();


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }
    }
}