using NUnit.Framework;
using VesteEVolta.Application.DTOs;
using VesteEVolta.DTO;

namespace Veste_e_Volta.Tests.DTO;

[TestFixture]
public class DTOValidationTests
{
    #region CreateClothingDto Tests

    [Test]
    public void CreateClothingDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new CreateClothingDto
        {
            Description = "Vestido Azul",
            RentPrice = 150.00m
        };

        // Assert
        Assert.That(dto.Description, Is.EqualTo("Vestido Azul"));
        Assert.That(dto.RentPrice, Is.EqualTo(150.00m));
    }

    [Test]
    public void CreateClothingDto_NullDescription_AllowsNull()
    {
        // Arrange & Act
        var dto = new CreateClothingDto
        {
            Description = null!,
            RentPrice = 100.00m
        };

        // Assert
        Assert.That(dto.Description, Is.Null);
    }

    [Test]
    public void CreateClothingDto_ZeroRentPrice_AllowsZero()
    {
        // Arrange & Act
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = 0
        };

        // Assert
        Assert.That(dto.RentPrice, Is.EqualTo(0));
    }

    [Test]
    public void CreateClothingDto_NegativeRentPrice_AllowsNegative()
    {
        // Arrange & Act
        var dto = new CreateClothingDto
        {
            Description = "Vestido",
            RentPrice = -100.00m
        };

        // Assert
        Assert.That(dto.RentPrice, Is.EqualTo(-100.00m));
    }

    #endregion

    #region ClothingUpdateDto Tests

    [Test]
    public void ClothingUpdateDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new ClothingUpdateDto
        {
            Description = "Vestido Atualizado",
            RentPrice = 200.00m,
            AvailabilityStatus = "RENTED"
        };

        // Assert
        Assert.That(dto.Description, Is.EqualTo("Vestido Atualizado"));
        Assert.That(dto.RentPrice, Is.EqualTo(200.00m));
        Assert.That(dto.AvailabilityStatus, Is.EqualTo("RENTED"));
    }

    [Test]
    public void ClothingUpdateDto_DefaultAvailabilityStatus_IsAvailable()
    {
        // Arrange & Act
        var dto = new ClothingUpdateDto
        {
            Description = "Vestido",
            RentPrice = 100.00m
        };

        // Assert
        Assert.That(dto.AvailabilityStatus, Is.EqualTo("AVAILABLE"));
    }

    [Test]
    public void ClothingUpdateDto_VariousStatuses_AcceptsAllValues()
    {
        // Arrange & Act
        var dto1 = new ClothingUpdateDto { Description = "Test", RentPrice = 100m, AvailabilityStatus = "AVAILABLE" };
        var dto2 = new ClothingUpdateDto { Description = "Test", RentPrice = 100m, AvailabilityStatus = "RENTED" };
        var dto3 = new ClothingUpdateDto { Description = "Test", RentPrice = 100m, AvailabilityStatus = "UNAVAILABLE" };

        // Assert
        Assert.That(dto1.AvailabilityStatus, Is.EqualTo("AVAILABLE"));
        Assert.That(dto2.AvailabilityStatus, Is.EqualTo("RENTED"));
        Assert.That(dto3.AvailabilityStatus, Is.EqualTo("UNAVAILABLE"));
    }

    #endregion

    #region CategoryRequestDto Tests

    [Test]
    public void CategoryRequestDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new CategoryRequestDto
        {
            Name = "Vestidos"
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo("Vestidos"));
    }

    [Test]
    public void CategoryRequestDto_EmptyName_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new CategoryRequestDto
        {
            Name = ""
        };

        // Assert
        Assert.That(dto.Name, Is.Empty);
    }

    [Test]
    public void CategoryRequestDto_WhitespaceName_AllowsWhitespace()
    {
        // Arrange & Act
        var dto = new CategoryRequestDto
        {
            Name = "   "
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo("   "));
    }

    [Test]
    public void CategoryRequestDto_LongName_AllowsLongStrings()
    {
        // Arrange & Act
        var longName = new string('A', 500);
        var dto = new CategoryRequestDto
        {
            Name = longName
        };

        // Assert
        Assert.That(dto.Name, Is.EqualTo(longName));
        Assert.That(dto.Name.Length, Is.EqualTo(500));
    }

    #endregion

    #region RatingDto Tests

    [Test]
    public void RatingDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var rentId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var createdAt = DateTime.UtcNow;

        // Act
        var dto = new RatingDto
        {
            Id = id,
            UserId = userId,
            RentId = rentId,
            ClothingId = clothingId,
            Rating = 5,
            Comment = "Excelente!",
            Date = date,
            CreatedAt = createdAt
        };

        // Assert
        Assert.That(dto.Id, Is.EqualTo(id));
        Assert.That(dto.UserId, Is.EqualTo(userId));
        Assert.That(dto.RentId, Is.EqualTo(rentId));
        Assert.That(dto.ClothingId, Is.EqualTo(clothingId));
        Assert.That(dto.Rating, Is.EqualTo(5));
        Assert.That(dto.Comment, Is.EqualTo("Excelente!"));
        Assert.That(dto.Date, Is.EqualTo(date));
        Assert.That(dto.CreatedAt, Is.EqualTo(createdAt));
    }

    [Test]
    public void RatingDto_NullComment_AllowsNull()
    {
        // Arrange & Act
        var dto = new RatingDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RentId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Rating = 3,
            Comment = null,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.That(dto.Comment, Is.Null);
    }

    [Test]
    public void RatingDto_EmptyComment_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new RatingDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RentId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Rating = 4,
            Comment = "",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.That(dto.Comment, Is.Empty);
    }

    [Test]
    public void RatingDto_VariousRatingValues_AcceptsAllIntegers()
    {
        // Arrange & Act
        var dto1 = CreateBasicRatingDto(1);
        var dto2 = CreateBasicRatingDto(3);
        var dto3 = CreateBasicRatingDto(5);
        var dto4 = CreateBasicRatingDto(0);
        var dto5 = CreateBasicRatingDto(-1);
        var dto6 = CreateBasicRatingDto(100);

        // Assert
        Assert.That(dto1.Rating, Is.EqualTo(1));
        Assert.That(dto2.Rating, Is.EqualTo(3));
        Assert.That(dto3.Rating, Is.EqualTo(5));
        Assert.That(dto4.Rating, Is.EqualTo(0));
        Assert.That(dto5.Rating, Is.EqualTo(-1));
        Assert.That(dto6.Rating, Is.EqualTo(100));
    }

    private RatingDto CreateBasicRatingDto(int rating)
    {
        return new RatingDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RentId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Rating = rating,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };
    }

    [Test]
    public void RatingDto_LongComment_AllowsLongStrings()
    {
        // Arrange
        var longComment = new string('X', 1000);

        // Act
        var dto = new RatingDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RentId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Rating = 5,
            Comment = longComment,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.That(dto.Comment, Is.EqualTo(longComment));
        Assert.That(dto.Comment!.Length, Is.EqualTo(1000));
    }

    [Test]
    public void RatingDto_SpecialCharactersInComment_PreservesCharacters()
    {
        // Arrange
        var specialComment = "Teste com \"aspas\", vírgulas, e caracteres especiais: @#$%^&*()";

        // Act
        var dto = new RatingDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            RentId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            Rating = 4,
            Comment = specialComment,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.That(dto.Comment, Is.EqualTo(specialComment));
    }

    #endregion

    #region CategoryResponseDto Tests

    [Test]
    public void CategoryResponseDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var dto = new CategoryResponseDto
        {
            CategoryId = categoryId,
            Name = "Vestidos Elegantes"
        };

        // Assert
        Assert.That(dto.CategoryId, Is.EqualTo(categoryId));
        Assert.That(dto.Name, Is.EqualTo("Vestidos Elegantes"));
    }

    [Test]
    public void CategoryResponseDto_EmptyGuid_AllowsEmptyGuid()
    {
        // Arrange & Act
        var dto = new CategoryResponseDto
        {
            CategoryId = Guid.Empty,
            Name = "Test"
        };

        // Assert
        Assert.That(dto.CategoryId, Is.EqualTo(Guid.Empty));
    }

    #endregion

    #region ClothingResponseDto Tests

    [Test]
    public void ClothingResponseDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var dto = new ClothingResponseDto
        {
            Id = id,
            Description = "Vestido Azul",
            RentPrice = 150.00m,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = ownerId,
            CreatedAt = createdAt
        };

        // Assert
        Assert.That(dto.Id, Is.EqualTo(id));
        Assert.That(dto.Description, Is.EqualTo("Vestido Azul"));
        Assert.That(dto.RentPrice, Is.EqualTo(150.00m));
        Assert.That(dto.AvailabilityStatus, Is.EqualTo("AVAILABLE"));
        Assert.That(dto.OwnerId, Is.EqualTo(ownerId));
        Assert.That(dto.CreatedAt, Is.EqualTo(createdAt));
    }

    #endregion
}
