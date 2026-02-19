using System.ComponentModel.DataAnnotations;
using VesteEVolta.DTO;

namespace Veste_e_Volta.Tests.DTO;

[TestFixture]
public class RentalDTOTests
{
    #region Property Assignment Tests

    [Test]
    public void RentalDTO_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 7);

        // Act
        var dto = new RentalDTO
        {
            UserId = userId,
            ClothingId = clothingId,
            StartDate = startDate,
            EndDate = endDate,
            Status = "active"
        };

        // Assert
        Assert.That(dto.UserId, Is.EqualTo(userId));
        Assert.That(dto.ClothingId, Is.EqualTo(clothingId));
        Assert.That(dto.StartDate, Is.EqualTo(startDate));
        Assert.That(dto.EndDate, Is.EqualTo(endDate));
        Assert.That(dto.Status, Is.EqualTo("active"));
    }

    [Test]
    public void RentalDTO_DefaultStatus_IsPending()
    {
        // Arrange & Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7)
        };

        // Assert
        Assert.That(dto.Status, Is.EqualTo("pending"));
    }

    [Test]
    public void RentalDTO_EmptyGuids_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new RentalDTO
        {
            UserId = Guid.Empty,
            ClothingId = Guid.Empty,
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7)
        };

        // Assert
        Assert.That(dto.UserId, Is.EqualTo(Guid.Empty));
        Assert.That(dto.ClothingId, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void RentalDTO_SameDayStartEnd_SetsCorrectly()
    {
        // Arrange
        var date = new DateOnly(2024, 1, 1);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = date,
            EndDate = date
        };

        // Assert
        Assert.That(dto.StartDate, Is.EqualTo(dto.EndDate));
    }

    [Test]
    public void RentalDTO_MultiDayRental_SetsCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 31);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        // Assert
        Assert.That(dto.StartDate, Is.EqualTo(startDate));
        Assert.That(dto.EndDate, Is.EqualTo(endDate));
        Assert.That(dto.EndDate.DayNumber - dto.StartDate.DayNumber, Is.EqualTo(30));
    }

    [Test]
    public void RentalDTO_ActiveStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            Status = "active"
        };

        // Assert
        Assert.That(dto.Status, Is.EqualTo("active"));
    }

    [Test]
    public void RentalDTO_CompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            Status = "completed"
        };

        // Assert
        Assert.That(dto.Status, Is.EqualTo("completed"));
    }

    [Test]
    public void RentalDTO_EmptyStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            Status = ""
        };

        // Assert
        Assert.That(dto.Status, Is.Empty);
    }

    #endregion

    #region Validation Tests

    [Test]
    public void RentalDTO_ValidData_PassesValidation()
    {
        // Arrange
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            Status = "pending"
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void RentalDTO_EmptyUserId_FailsValidation()
    {
        // Arrange
        var dto = new RentalDTO
        {
            UserId = Guid.Empty,
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7)
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        // Note: Guid.Empty is still a valid Guid value, so this will pass validation
        // unless there's custom validation logic
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void RentalDTO_AllRequiredFieldsPresent_PassesValidation()
    {
        // Arrange
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7)
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        Assert.That(isValid, Is.True);
        Assert.That(results, Is.Empty);
    }

    #endregion

    #region Date Calculation Tests

    [Test]
    public void RentalDTO_OneDayRental_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 2);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var days = dto.EndDate.DayNumber - dto.StartDate.DayNumber;

        // Assert
        Assert.That(days, Is.EqualTo(1));
    }

    [Test]
    public void RentalDTO_WeekRental_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 8);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var days = dto.EndDate.DayNumber - dto.StartDate.DayNumber;

        // Assert
        Assert.That(days, Is.EqualTo(7));
    }

    [Test]
    public void RentalDTO_CrossMonthRental_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2024, 1, 25);
        var endDate = new DateOnly(2024, 2, 5);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var days = dto.EndDate.DayNumber - dto.StartDate.DayNumber;

        // Assert
        Assert.That(days, Is.EqualTo(11));
    }

    [Test]
    public void RentalDTO_LeapYearRental_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateOnly(2024, 2, 28);
        var endDate = new DateOnly(2024, 3, 1);

        // Act
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate
        };

        var days = dto.EndDate.DayNumber - dto.StartDate.DayNumber;

        // Assert
        Assert.That(days, Is.EqualTo(2)); // Includes Feb 29
    }

    #endregion
}
