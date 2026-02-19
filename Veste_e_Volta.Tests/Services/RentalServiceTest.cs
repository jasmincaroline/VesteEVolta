using Moq;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;

namespace VesteEVolta.Tests.Services
{
    [TestFixture]
    public class RatingServiceTests
    {
        private Mock<IRatingRepository> _ratingRepositoryMock = null!;
        private Mock<IRentalRepository> _rentalRepositoryMock = null!;
        private RatingService _ratingService = null!;

        [SetUp]
        public void Setup()
        {
            _ratingRepositoryMock = new Mock<IRatingRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _ratingService = new RatingService(_ratingRepositoryMock.Object, _rentalRepositoryMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateRating_WhenValid()
        {
            // Arrange
            var rentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var clothingId = Guid.NewGuid();

            var rent = new TbRental
            {
                Id = rentId,
                UserId = userId,
                ClothingId = clothingId,
                Status = "Finished",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var dto = new RatingDto
            {
                RentId = rentId,
                UserId = userId,
                ClothingId = clothingId,
                Rating = 5,
                Comment = "Excelente"
            };

            _rentalRepositoryMock.Setup(r => r.GetById(rentId))
                .ReturnsAsync(rent);

            _ratingRepositoryMock.Setup(r => r.GetByRentIdAsync(rentId))
                .ReturnsAsync((TbRating?)null);

            _ratingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TbRating>()))
                .Returns(Task.CompletedTask);

            // Act
            await _ratingService.CreateAsync(dto);

            // Assert
            _ratingRepositoryMock.Verify(r => r.AddAsync(It.Is<TbRating>(
                t => t.RentId == rentId &&
                     t.UserId == userId &&
                     t.ClothingId == clothingId &&
                     t.Rating == 5 &&
                     t.Comment == "Excelente"
            )), Times.Once);
        }

        [Test]
        public void CreateAsync_ShouldThrow_WhenRentalNotFound()
        {
            // Arrange
            var dto = new RatingDto
            {
                RentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 4,
                Comment = "Bom"
            };

            _rentalRepositoryMock.Setup(r => r.GetById(dto.RentId))
                .ReturnsAsync((TbRental?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _ratingService.CreateAsync(dto)
            );
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado."));
        }

        [Test]
        public async Task GetByUserAsync_ShouldReturnRatings()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var rating = new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RentId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Ótimo",
                CreatedAt = DateTime.UtcNow,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _ratingRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<TbRating> { rating });

            // Act
            var result = await _ratingService.GetByUserAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetByClothingAsync_ShouldReturnRatings()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var rating = new TbRating
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RentId = Guid.NewGuid(),
                ClothingId = clothingId,
                Rating = 4,
                Comment = "Bom",
                CreatedAt = DateTime.UtcNow,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _ratingRepositoryMock.Setup(r => r.GetByClothingIdAsync(clothingId))
                .ReturnsAsync(new List<TbRating> { rating });

            // Act
            var result = await _ratingService.GetByClothingAsync(clothingId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ClothingId, Is.EqualTo(clothingId));
        }

        [Test]
        public void DeleteAsync_ShouldThrow_WhenRatingNotFound()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _ratingRepositoryMock.Setup(r => r.GetByIdAsync(ratingId))
                .ReturnsAsync((TbRating?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _ratingService.DeleteAsync(ratingId, userId)
            );
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Is.EqualTo("Avaliação não encontrada."));
        }
    }
}
