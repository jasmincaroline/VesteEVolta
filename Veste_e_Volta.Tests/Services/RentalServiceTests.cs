using Moq;
using NUnit.Framework;
using VesteEVolta.DTO;           
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;


namespace VesteEVolta.Tests.Services
{
    [TestFixture]
    public class RentalServiceTests
    {
        private Mock<IRentalRepository> _rentalRepositoryMock = null!;
        private RentalService _service = null!;

        [SetUp]
        public void Setup()
        {
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _service = new RentalService(_rentalRepositoryMock.Object);
        }

        [Test]
        public async Task Create_ShouldCreateRental_AndReturnResponse()
        {
            // Arrange
            var dto = new RentalDTO
            {
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            };

            _rentalRepositoryMock
                .Setup(r => r.Add(It.IsAny<TbRental>()))
                .ReturnsAsync((TbRental rental) => rental);

            // Act
            var result = await _service.Create(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(dto.UserId));
            Assert.That(result.ClothingId, Is.EqualTo(dto.ClothingId));
            Assert.That(result.Status, Is.EqualTo("active"));

            _rentalRepositoryMock.Verify(r => r.Add(It.Is<TbRental>(t =>
                t.UserId == dto.UserId &&
                t.ClothingId == dto.ClothingId &&
                t.Status == "active" &&
                t.TotalValue == 0
            )), Times.Once);
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _rentalRepositoryMock.Setup(r => r.GetById(id))
                .ReturnsAsync((TbRental?)null);

            // Act
            var result = await _service.GetById(id);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateStatus_ShouldThrow_WhenRentalNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _rentalRepositoryMock.Setup(r => r.GetById(id))
                .ReturnsAsync((TbRental?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateStatus(id, "canceled")
            );

            Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado"));
        }

        [Test]
        public async Task UpdateStatus_ShouldUpdate_WhenRentalExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var rental = new TbRental
            {
                Id = id,
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Status = "active",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                CreatedAt = DateTime.UtcNow
            };

            _rentalRepositoryMock.Setup(r => r.GetById(id))
                .ReturnsAsync(rental);

            _rentalRepositoryMock.Setup(r => r.Update(It.IsAny<TbRental>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateStatus(id, "canceled");

            // Assert
            Assert.That(rental.Status, Is.EqualTo("canceled"));
            _rentalRepositoryMock.Verify(r => r.Update(It.Is<TbRental>(t =>
                t.Id == id && t.Status == "canceled"
            )), Times.Once);
        }

        [Test]
        public async Task GetById_ShouldReturnDto_WhenFound()
        {
            var id = Guid.NewGuid();
            var rental = new TbRental
            {
                Id = id,
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Status = "active",
                TotalValue = 10,
                CreatedAt = DateTime.UtcNow
            };

            _rentalRepositoryMock.Setup(r => r.GetById(id)).ReturnsAsync(rental);

            var result = await _service.GetById(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.Status, Is.EqualTo("active"));
        }

        [Test]
        public async Task GetAll_ShouldReturnMappedDtos()
        {
            var rentals = new List<TbRental>
    {
        new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ClothingId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Status = "active",
            TotalValue = 0,
            CreatedAt = DateTime.UtcNow
        }
    };

            _rentalRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(rentals);

            var result = (await _service.GetAll()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Status, Is.EqualTo("active"));
        }

        [Test]
        public async Task GetByUserId_ShouldReturnMappedDtos()
        {
            var userId = Guid.NewGuid();

            _rentalRepositoryMock.Setup(r => r.GetByUserId(userId))
                .ReturnsAsync(new List<TbRental>
                {
            new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClothingId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Status = "active",
                TotalValue = 0,
                CreatedAt = DateTime.UtcNow
            }
                });

            var result = (await _service.GetByUserId(userId)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo(userId));
        }


    }
}
