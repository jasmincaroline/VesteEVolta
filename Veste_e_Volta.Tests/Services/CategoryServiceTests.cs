using Moq;
using NUnit.Framework;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Services;

[TestFixture]
public class CategoryServiceTests
{
    private Mock<ICategoryRepositories> _mockCategoryRepository = default!;
    private CategoryService _service = default!;

    [SetUp]
    public void Setup()
    {
        _mockCategoryRepository = new Mock<ICategoryRepositories>();
        _service = new CategoryService(_mockCategoryRepository.Object);
    }

    #region UpdateCategory Tests

    [Test]
    public void UpdateCategory_ValidData_ReturnsCategoryResponseDto()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos"
        };

        var dto = new CategoryRequestDto
        {
            Name = "Vestidos Elegantes"
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(existingCategory);

        _mockCategoryRepository
            .Setup(r => r.GetByName(dto.Name))
            .Returns((TbCategory?)null);

        _mockCategoryRepository
            .Setup(r => r.Update(It.IsAny<TbCategory>()));

        _mockCategoryRepository
            .Setup(r => r.SaveChanges());

        // Act
        var result = _service.UpdateCategory(categoryId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CategoryId, Is.EqualTo(categoryId));
        Assert.That(result.Name, Is.EqualTo("Vestidos Elegantes"));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.GetByName(dto.Name), Times.Once);
        _mockCategoryRepository.Verify(r => r.Update(It.Is<TbCategory>(c => 
            c.CategoryId == categoryId && c.Name == "Vestidos Elegantes")), Times.Once);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Test]
    public void UpdateCategory_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new CategoryRequestDto
        {
            Name = "Nova Categoria"
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns((TbCategory?)null);

        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() =>
            _service.UpdateCategory(categoryId, dto));

        Assert.That(ex!.Message, Does.Contain("Categoria não encontrada :("));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Update(It.IsAny<TbCategory>()), Times.Never);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Test]
    public void UpdateCategory_DuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();

        var existingCategory = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos"
        };

        var duplicateCategory = new TbCategory
        {
            CategoryId = otherCategoryId,
            Name = "Saias"
        };

        var dto = new CategoryRequestDto
        {
            Name = "Saias"
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(existingCategory);

        _mockCategoryRepository
            .Setup(r => r.GetByName(dto.Name))
            .Returns(duplicateCategory);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _service.UpdateCategory(categoryId, dto));

        Assert.That(ex!.Message, Does.Contain("Já existe uma categoria com esse nome"));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.GetByName(dto.Name), Times.Once);
        _mockCategoryRepository.Verify(r => r.Update(It.IsAny<TbCategory>()), Times.Never);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Test]
    public void UpdateCategory_SameNameSameCategory_UpdatesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos"
        };

        var dto = new CategoryRequestDto
        {
            Name = "Vestidos"
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(existingCategory);

        _mockCategoryRepository
            .Setup(r => r.GetByName(dto.Name))
            .Returns(existingCategory); // Mesma categoria

        // Act
        var result = _service.UpdateCategory(categoryId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CategoryId, Is.EqualTo(categoryId));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Update(It.IsAny<TbCategory>()), Times.Once);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Test]
    public void UpdateCategory_NameWithWhitespace_TrimsName()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos"
        };

        var dto = new CategoryRequestDto
        {
            Name = "   Roupas de Festa   "
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(existingCategory);

        _mockCategoryRepository
            .Setup(r => r.GetByName(dto.Name))
            .Returns((TbCategory?)null);

        // Act
        var result = _service.UpdateCategory(categoryId, dto);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Roupas de Festa"));

        _mockCategoryRepository.Verify(r => r.Update(It.Is<TbCategory>(c => 
            c.Name == "Roupas de Festa")), Times.Once);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    #endregion

    #region DeleteCategory Tests

    [Test]
    public void DeleteCategory_ValidCategory_DeletesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos",
            Clothings = new List<TbClothing>()
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(category);

        _mockCategoryRepository
            .Setup(r => r.Delete(category));

        _mockCategoryRepository
            .Setup(r => r.SaveChanges());

        // Act
        _service.DeleteCategory(categoryId);

        // Assert
        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Delete(category), Times.Once);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Test]
    public void DeleteCategory_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns((TbCategory?)null);

        // Act & Assert
        var ex = Assert.Throws<KeyNotFoundException>(() =>
            _service.DeleteCategory(categoryId));

        Assert.That(ex!.Message, Does.Contain("Categoria não encontrada :("));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Delete(It.IsAny<TbCategory>()), Times.Never);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Test]
    public void DeleteCategory_CategoryWithClothings_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos",
            Clothings = new List<TbClothing>
            {
                new TbClothing
                {
                    Id = Guid.NewGuid(),
                    Description = "Vestido Azul",
                    RentPrice = 100.00m
                }
            }
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(category);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _service.DeleteCategory(categoryId));

        Assert.That(ex!.Message, Does.Contain("essa categoria possui roupas vinculadas"));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Delete(It.IsAny<TbCategory>()), Times.Never);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Test]
    public void DeleteCategory_CategoryWithMultipleClothings_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos",
            Clothings = new List<TbClothing>
            {
                new TbClothing { Id = Guid.NewGuid(), Description = "Vestido 1", RentPrice = 100m },
                new TbClothing { Id = Guid.NewGuid(), Description = "Vestido 2", RentPrice = 150m },
                new TbClothing { Id = Guid.NewGuid(), Description = "Vestido 3", RentPrice = 200m }
            }
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(category);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _service.DeleteCategory(categoryId));

        Assert.That(ex!.Message, Does.Contain("essa categoria possui roupas vinculadas"));

        _mockCategoryRepository.Verify(r => r.GetById(categoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.Delete(It.IsAny<TbCategory>()), Times.Never);
    }

    [Test]
    public void DeleteCategory_CategoryWithNullClothings_DeletesSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new TbCategory
        {
            CategoryId = categoryId,
            Name = "Vestidos",
            Clothings = null!
        };

        _mockCategoryRepository
            .Setup(r => r.GetById(categoryId))
            .Returns(category);

        // Act
        _service.DeleteCategory(categoryId);

        // Assert
        _mockCategoryRepository.Verify(r => r.Delete(category), Times.Once);
        _mockCategoryRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    #endregion
}
