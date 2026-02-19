using Moq;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Services;

[TestFixture]
public class RentalServiceTests
{
    private Mock<IRentalRepository> _rentalRepositoryMock;
    private Mock<IClothingRepository> _clothingRepositoryMock;
    private RentalService _service;

    [SetUp]
    public void Setup()
    {
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _clothingRepositoryMock = new Mock<IClothingRepository>();
        _service = new RentalService(_rentalRepositoryMock.Object, _clothingRepositoryMock.Object);
    }

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsRentalResponseDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clothingId = Guid.NewGuid();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 8);

        var dto = new RentalDTO
        {
            UserId = userId,
            ClothingId = clothingId,
            StartDate = startDate,
            EndDate = endDate,
            Status = "pending"
        };

        var clothing = new TbClothing
        {
            Id = clothingId,
            RentPrice = 50.00m
        };

        var expectedRental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ClothingId = clothingId,
            StartDate = startDate,
            EndDate = endDate,
            Status = "active",
            TotalValue = 350.00m,
            CreatedAt = DateTime.UtcNow
        };

        _clothingRepositoryMock
            .Setup(c => c.GetByIdAsync(clothingId))
            .ReturnsAsync(clothing);

        _rentalRepositoryMock
            .Setup(r => r.Add(It.IsAny<TbRental>()))
            .ReturnsAsync(expectedRental);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.ClothingId, Is.EqualTo(clothingId));
        Assert.That(result.StartDate, Is.EqualTo(startDate));
        Assert.That(result.EndDate, Is.EqualTo(endDate));
        Assert.That(result.Status, Is.EqualTo("active"));

        _clothingRepositoryMock.Verify(c => c.GetByIdAsync(clothingId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Add(It.IsAny<TbRental>()), Times.Once);
    }

    [Test]
    public async Task Create_CalculatesTotalValue_Correctly()
    {
        // Arrange
        var clothingId = Guid.NewGuid();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 6);

        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = clothingId,
            StartDate = startDate,
            EndDate = endDate
        };

        var clothing = new TbClothing
        {
            Id = clothingId,
            RentPrice = 100.00m
        };

        var expectedTotalValue = 5 * 100.00m; // 5 days

        _clothingRepositoryMock
            .Setup(c => c.GetByIdAsync(clothingId))
            .ReturnsAsync(clothing);

        _rentalRepositoryMock
            .Setup(r => r.Add(It.IsAny<TbRental>()))
            .ReturnsAsync((TbRental r) => r);

        // Act
        var result = await _service.Create(dto);

        // Assert
        _rentalRepositoryMock.Verify(r => r.Add(It.Is<TbRental>(rental =>
            rental.TotalValue == expectedTotalValue)), Times.Once);
    }

    [Test]
    public void Create_ClothingNotFound_ThrowsException()
    {
        // Arrange
        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7)
        };

        _clothingRepositoryMock
            .Setup(c => c.GetByIdAsync(dto.ClothingId))
            .ReturnsAsync((TbClothing?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Create(dto));
        Assert.That(ex!.Message, Is.EqualTo("Roupa não encontrada."));

        _clothingRepositoryMock.Verify(c => c.GetByIdAsync(dto.ClothingId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Add(It.IsAny<TbRental>()), Times.Never);
    }

    [Test]
    public async Task Create_OneDayRental_CalculatesCorrectly()
    {
        // Arrange
        var clothingId = Guid.NewGuid();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 2);

        var dto = new RentalDTO
        {
            UserId = Guid.NewGuid(),
            ClothingId = clothingId,
            StartDate = startDate,
            EndDate = endDate
        };

        var clothing = new TbClothing
        {
            Id = clothingId,
            RentPrice = 50.00m
        };

        _clothingRepositoryMock
            .Setup(c => c.GetByIdAsync(clothingId))
            .ReturnsAsync(clothing);

        _rentalRepositoryMock
            .Setup(r => r.Add(It.IsAny<TbRental>()))
            .ReturnsAsync((TbRental r) => r);

        // Act
        await _service.Create(dto);

        // Assert
        _rentalRepositoryMock.Verify(r => r.Add(It.Is<TbRental>(rental =>
            rental.TotalValue == 50.00m)), Times.Once);
    }

    [Test]
    public async Task Create_SetsActiveStatus_Always()
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

        var clothing = new TbClothing
        {
            Id = dto.ClothingId,
            RentPrice = 50.00m
        };

        _clothingRepositoryMock
            .Setup(c => c.GetByIdAsync(dto.ClothingId))
            .ReturnsAsync(clothing);

        _rentalRepositoryMock
            .Setup(r => r.Add(It.IsAny<TbRental>()))
            .ReturnsAsync((TbRental r) => r);

        // Act
        await _service.Create(dto);

        // Assert
        _rentalRepositoryMock.Verify(r => r.Add(It.Is<TbRental>(rental =>
            rental.Status == "active")), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Test]
    public async Task GetById_ExistingId_ReturnsRentalResponseDTO()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new TbRental
        {
            Id = rentalId,
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            TotalValue = 350.00m,
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync(rental);

        // Act
        var result = await _service.GetById(rentalId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(rentalId));
        Assert.That(result.UserId, Is.EqualTo(rental.UserId));
        Assert.That(result.ClothingId, Is.EqualTo(rental.ClothingId));
        Assert.That(result.TotalValue, Is.EqualTo(rental.TotalValue));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
    }

    [Test]
    public void GetById_NonExistingId_ThrowsException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync((TbRental?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetById(rentalId));
        Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado."));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
    }

    #endregion

    #region GetAll Tests

    [Test]
    public async Task GetAll_RentalsExist_ReturnsListOfRentals()
    {
        // Arrange
        var rentals = new List<TbRental>
        {
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 1, 7),
                TotalValue = 350.00m,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            },
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                StartDate = new DateOnly(2024, 2, 1),
                EndDate = new DateOnly(2024, 2, 7),
                TotalValue = 450.00m,
                Status = "completed",
                CreatedAt = DateTime.UtcNow
            }
        };

        _rentalRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(rentals);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));

        _rentalRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetAll_NoRentals_ReturnsEmptyList()
    {
        // Arrange
        _rentalRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<TbRental>());

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        _rentalRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    #endregion

    #region GetByUserId Tests

    [Test]
    public async Task GetByUserId_RentalsExist_ReturnsListOfRentals()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rentals = new List<TbRental>
        {
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClothingId = Guid.NewGuid(),
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 1, 7),
                TotalValue = 350.00m,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            },
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClothingId = Guid.NewGuid(),
                StartDate = new DateOnly(2024, 2, 1),
                EndDate = new DateOnly(2024, 2, 7),
                TotalValue = 450.00m,
                Status = "completed",
                CreatedAt = DateTime.UtcNow
            }
        };

        _rentalRepositoryMock
            .Setup(r => r.GetByUserId(userId))
            .ReturnsAsync(rentals);

        // Act
        var result = await _service.GetByUserId(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(r => r.UserId == userId), Is.True);

        _rentalRepositoryMock.Verify(r => r.GetByUserId(userId), Times.Once);
    }

    [Test]
    public async Task GetByUserId_NoRentals_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _rentalRepositoryMock
            .Setup(r => r.GetByUserId(userId))
            .ReturnsAsync(new List<TbRental>());

        // Act
        var result = await _service.GetByUserId(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        _rentalRepositoryMock.Verify(r => r.GetByUserId(userId), Times.Once);
    }

    #endregion

    #region GetByClothingId Tests

    [Test]
    public async Task GetByClothingId_RentalsExist_ReturnsListOfRentals()
    {
        // Arrange
        var clothingId = Guid.NewGuid();
        var rentals = new List<TbRental>
        {
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = clothingId,
                StartDate = new DateOnly(2024, 1, 1),
                EndDate = new DateOnly(2024, 1, 7),
                TotalValue = 350.00m,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            }
        };

        _rentalRepositoryMock
            .Setup(r => r.GetByClothingId(clothingId))
            .ReturnsAsync(rentals);

        // Act
        var result = await _service.GetByClothingId(clothingId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.All(r => r.ClothingId == clothingId), Is.True);

        _rentalRepositoryMock.Verify(r => r.GetByClothingId(clothingId), Times.Once);
    }

    [Test]
    public async Task GetByClothingId_NoRentals_ReturnsEmptyList()
    {
        // Arrange
        var clothingId = Guid.NewGuid();

        _rentalRepositoryMock
            .Setup(r => r.GetByClothingId(clothingId))
            .ReturnsAsync(new List<TbRental>());

        // Act
        var result = await _service.GetByClothingId(clothingId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        _rentalRepositoryMock.Verify(r => r.GetByClothingId(clothingId), Times.Once);
    }

    #endregion

    #region UpdateStatus Tests

    [Test]
    public async Task UpdateStatus_ExistingRental_UpdatesSuccessfully()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new TbRental
        {
            Id = rentalId,
            Status = "active"
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync(rental);

        _rentalRepositoryMock
            .Setup(r => r.Update(It.IsAny<TbRental>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateStatus(rentalId, "completed");

        // Assert
        Assert.That(rental.Status, Is.EqualTo("completed"));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Update(rental), Times.Once);
    }

    [Test]
    public void UpdateStatus_NonExistingRental_ThrowsException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync((TbRental?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.UpdateStatus(rentalId, "completed"));

        Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado"));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Update(It.IsAny<TbRental>()), Times.Never);
    }

    [Test]
    public async Task UpdateStatus_ToCancelled_UpdatesSuccessfully()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new TbRental
        {
            Id = rentalId,
            Status = "active"
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync(rental);

        _rentalRepositoryMock
            .Setup(r => r.Update(It.IsAny<TbRental>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateStatus(rentalId, "cancelled");

        // Assert
        Assert.That(rental.Status, Is.EqualTo("cancelled"));
        _rentalRepositoryMock.Verify(r => r.Update(rental), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_ExistingRental_DeletesAndReturnsDTO()
    {
        // Arrange
        var rentalId = Guid.NewGuid();
        var rental = new TbRental
        {
            Id = rentalId,
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 1, 7),
            TotalValue = 350.00m,
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync(rental);

        _rentalRepositoryMock
            .Setup(r => r.Delete(rentalId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Delete(rentalId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(rentalId));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Delete(rentalId), Times.Once);
    }

    [Test]
    public void Delete_NonExistingRental_ThrowsException()
    {
        // Arrange
        var rentalId = Guid.NewGuid();

        _rentalRepositoryMock
            .Setup(r => r.GetById(rentalId))
            .ReturnsAsync((TbRental?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _service.Delete(rentalId));
        Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado"));

        _rentalRepositoryMock.Verify(r => r.GetById(rentalId), Times.Once);
        _rentalRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
}
