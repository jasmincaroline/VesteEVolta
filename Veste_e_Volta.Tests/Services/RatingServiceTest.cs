using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;

namespace VesteEVolta.Tests.Services
{
    [TestFixture]
    public class RatingServiceTests
    {
        private Mock<IRatingRepository> _ratingRepoMock = null!;
        private Mock<IRentalRepository> _rentalRepoMock = null!;
        private RatingService _ratingService = null!;

        [SetUp]
        public void Setup()
        {
            _ratingRepoMock = new Mock<IRatingRepository>();
            _rentalRepoMock = new Mock<IRentalRepository>();
            _ratingService = new RatingService(_ratingRepoMock.Object, _rentalRepoMock.Object);
        }

        #region CreateAsync Tests

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

            _rentalRepoMock.Setup(r => r.GetById(rentId)).ReturnsAsync(rent);
            _ratingRepoMock.Setup(r => r.GetByRentIdAsync(rentId)).ReturnsAsync((TbRating?)null);
            _ratingRepoMock.Setup(r => r.AddAsync(It.IsAny<TbRating>())).Returns(Task.CompletedTask);

            // Act
            await _ratingService.CreateAsync(dto);

            // Assert
            _ratingRepoMock.Verify(r => r.AddAsync(It.Is<TbRating>(
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

            _rentalRepoMock.Setup(r => r.GetById(dto.RentId)).ReturnsAsync((TbRental?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _ratingService.CreateAsync(dto)
            );
            Assert.That(ex!.Message, Is.EqualTo("Aluguel não encontrado."));
        }

        #endregion

        #region GetByUserAsync / GetByClothingAsync

        [Test]
        public async Task GetByUserAsync_ShouldReturnRatings()
        {
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

            _ratingRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<TbRating> { rating });

            var result = await _ratingService.GetByUserAsync(userId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetByClothingAsync_ShouldReturnRatings()
        {
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

            _ratingRepoMock.Setup(r => r.GetByClothingIdAsync(clothingId)).ReturnsAsync(new List<TbRating> { rating });

            var result = await _ratingService.GetByClothingAsync(clothingId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ClothingId, Is.EqualTo(clothingId));
        }

        #endregion
    }
}
