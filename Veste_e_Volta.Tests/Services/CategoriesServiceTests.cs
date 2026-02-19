using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Category
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepositories> _categoryRepositoryMock;
        private CategoryService _categoryService;

        [SetUp]
        public void Setup()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepositories>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object);
        }

        // GetCategories
  
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

        [Test]
        public void GetCategories_WhenThereAreCategories_ShouldMapToResponseDto()
        {
            // Arrange
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(repo => repo.GetAll())
                .Returns(new List<TbCategory>
                {
                    new TbCategory { CategoryId = id1, Name = "Vestidos" },
                    new TbCategory { CategoryId = id2, Name = "Bolsas" }
                });

            // Act
            var result = _categoryService.GetCategories();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].CategoryId, Is.EqualTo(id1));
            Assert.That(result[0].Name, Is.EqualTo("Vestidos"));
            Assert.That(result[1].CategoryId, Is.EqualTo(id2));
            Assert.That(result[1].Name, Is.EqualTo("Bolsas"));

            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        // UpdateCategory

        [Test]
        public void UpdateCategory_WhenCategoryNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CategoryRequestDto { Name = "Nova" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetById(id))
                .Returns((TbCategory?)null);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => _categoryService.UpdateCategory(id, dto));

            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetByName(It.IsAny<string>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.Update(It.IsAny<TbCategory>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Test]
        public void UpdateCategory_WhenNameAlreadyExistsForAnotherCategory_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var otherId = Guid.NewGuid();

            var current = new TbCategory { CategoryId = id, Name = "Antiga" };
            var existingWithSameName = new TbCategory { CategoryId = otherId, Name = "Vestidos" };

            var dto = new CategoryRequestDto { Name = "Vestidos" };

            _categoryRepositoryMock.Setup(repo => repo.GetById(id)).Returns(current);
            _categoryRepositoryMock.Setup(repo => repo.GetByName(dto.Name)).Returns(existingWithSameName);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => _categoryService.UpdateCategory(id, dto));

            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetByName(dto.Name), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Update(It.IsAny<TbCategory>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Test]
        public void UpdateCategory_WhenValid_ShouldUpdateAndSaveAndReturnDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var current = new TbCategory { CategoryId = id, Name = "Antiga" };

            var dto = new CategoryRequestDto { Name = "  Vestidos de Festa  " };

            _categoryRepositoryMock.Setup(repo => repo.GetById(id)).Returns(current);
            _categoryRepositoryMock.Setup(repo => repo.GetByName(dto.Name)).Returns((TbCategory?)null);

            // Act
            var result = _categoryService.UpdateCategory(id, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CategoryId, Is.EqualTo(id));
            Assert.That(result.Name, Is.EqualTo("Vestidos de Festa"));

            // Confere que trim foi aplicado no entity
            Assert.That(current.Name, Is.EqualTo("Vestidos de Festa"));

            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetByName(dto.Name), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Update(It.Is<TbCategory>(c => c.CategoryId == id && c.Name == "Vestidos de Festa")), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }


        // DeleteCategory

        [Test]
        public void DeleteCategory_WhenNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(repo => repo.GetById(id))
                .Returns((TbCategory?)null);

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => _categoryService.DeleteCategory(id));

            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<TbCategory>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Test]
        public void DeleteCategory_WhenHasLinkedClothings_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();

            var category = new TbCategory
            {
                CategoryId = id,
                Name = "Vestidos",
                Clothings = new List<TbClothing>
                {
                    new TbClothing { Id = Guid.NewGuid(), Description = "Vestido", RentPrice = 100, AvailabilityStatus = "AVAILABLE", OwnerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
                }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetById(id)).Returns(category);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => _categoryService.DeleteCategory(id));

            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<TbCategory>()), Times.Never);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Never);
        }

        [Test]
        public void DeleteCategory_WhenNoLinkedClothings_ShouldDeleteAndSave()
        {
            // Arrange
            var id = Guid.NewGuid();

            var category = new TbCategory
            {
                CategoryId = id,
                Name = "Bolsas",
                Clothings = new List<TbClothing>() // vazio
            };

            _categoryRepositoryMock.Setup(repo => repo.GetById(id)).Returns(category);

            // Act
            _categoryService.DeleteCategory(id);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.Delete(It.Is<TbCategory>(c => c.CategoryId == id)), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.SaveChanges(), Times.Once);
        }
    }
}
