using VesteEVolta.DTO;

namespace Veste_e_Volta.Tests.DTO;

[TestFixture]
public class PaymentResponseDtoTests
{
    #region Property Assignment Tests

    [Test]
    public void PaymentResponseDto_ValidData_SetsPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rentalId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var dto = new PaymentResponseDto
        {
            Id = id,
            RentalId = rentalId,
            PaymentMethod = "Credit Card",
            Amount = 150.50m,
            PaymentStatus = "completed",
            CreatedAt = createdAt
        };

        // Assert
        Assert.That(dto.Id, Is.EqualTo(id));
        Assert.That(dto.RentalId, Is.EqualTo(rentalId));
        Assert.That(dto.PaymentMethod, Is.EqualTo("Credit Card"));
        Assert.That(dto.Amount, Is.EqualTo(150.50m));
        Assert.That(dto.PaymentStatus, Is.EqualTo("completed"));
        Assert.That(dto.CreatedAt, Is.EqualTo(createdAt));
    }

    [Test]
    public void PaymentResponseDto_DefaultPaymentStatus_IsPending()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Debit Card",
            Amount = 100.00m
        };

        // Assert
        Assert.That(dto.PaymentStatus, Is.EqualTo("pending"));
    }

    [Test]
    public void PaymentResponseDto_EmptyPaymentMethod_AllowsEmpty()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "",
            Amount = 100.00m
        };

        // Assert
        Assert.That(dto.PaymentMethod, Is.Empty);
    }

    [Test]
    public void PaymentResponseDto_ZeroAmount_AllowsZero()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Cash",
            Amount = 0m
        };

        // Assert
        Assert.That(dto.Amount, Is.EqualTo(0m));
    }

    [Test]
    public void PaymentResponseDto_NegativeAmount_AllowsNegative()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Refund",
            Amount = -50.00m
        };

        // Assert
        Assert.That(dto.Amount, Is.EqualTo(-50.00m));
    }

    [Test]
    public void PaymentResponseDto_LargeAmount_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Wire Transfer",
            Amount = 999999.99m
        };

        // Assert
        Assert.That(dto.Amount, Is.EqualTo(999999.99m));
    }

    [Test]
    public void PaymentResponseDto_CreditCardPaymentMethod_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Credit Card",
            Amount = 200.00m
        };

        // Assert
        Assert.That(dto.PaymentMethod, Is.EqualTo("Credit Card"));
    }

    [Test]
    public void PaymentResponseDto_DebitCardPaymentMethod_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Debit Card",
            Amount = 150.00m
        };

        // Assert
        Assert.That(dto.PaymentMethod, Is.EqualTo("Debit Card"));
    }

    [Test]
    public void PaymentResponseDto_PixPaymentMethod_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "PIX",
            Amount = 100.00m
        };

        // Assert
        Assert.That(dto.PaymentMethod, Is.EqualTo("PIX"));
    }

    [Test]
    public void PaymentResponseDto_CompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Cash",
            Amount = 50.00m,
            PaymentStatus = "completed"
        };

        // Assert
        Assert.That(dto.PaymentStatus, Is.EqualTo("completed"));
    }

    [Test]
    public void PaymentResponseDto_FailedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Credit Card",
            Amount = 100.00m,
            PaymentStatus = "failed"
        };

        // Assert
        Assert.That(dto.PaymentStatus, Is.EqualTo("failed"));
    }

    [Test]
    public void PaymentResponseDto_PendingStatus_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Bank Transfer",
            Amount = 75.00m,
            PaymentStatus = "pending"
        };

        // Assert
        Assert.That(dto.PaymentStatus, Is.EqualTo("pending"));
    }

    [Test]
    public void PaymentResponseDto_EmptyGuids_SetsCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.Empty,
            RentalId = Guid.Empty,
            PaymentMethod = "Cash",
            Amount = 100.00m
        };

        // Assert
        Assert.That(dto.Id, Is.EqualTo(Guid.Empty));
        Assert.That(dto.RentalId, Is.EqualTo(Guid.Empty));
    }

    [Test]
    public void PaymentResponseDto_FutureCreatedAt_SetsCorrectly()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(10);

        // Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Cash",
            Amount = 100.00m,
            CreatedAt = futureDate
        };

        // Assert
        Assert.That(dto.CreatedAt, Is.EqualTo(futureDate));
    }

    [Test]
    public void PaymentResponseDto_PastCreatedAt_SetsCorrectly()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddYears(-1);

        // Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Credit Card",
            Amount = 200.00m,
            CreatedAt = pastDate
        };

        // Assert
        Assert.That(dto.CreatedAt, Is.EqualTo(pastDate));
    }

    [Test]
    public void PaymentResponseDto_DecimalPrecision_PreservesCorrectly()
    {
        // Arrange & Act
        var dto = new PaymentResponseDto
        {
            Id = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            PaymentMethod = "Credit Card",
            Amount = 123.456789m
        };

        // Assert
        Assert.That(dto.Amount, Is.EqualTo(123.456789m));
    }

    #endregion
}
