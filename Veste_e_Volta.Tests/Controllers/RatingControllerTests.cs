using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VesteEVolta.Controllers;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class RatingControllerTests
    {
        private Mock<IRatingService> _ratingServiceMock = null!;
        private RatingController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _ratingServiceMock = new Mock<IRatingService>();
            _controller = new RatingController(_ratingServiceMock.Object);
        }

        [Test]
        public async Task Create_ShouldReturnOk_WithSuccessMessage()
        {
            // Arrange
            var dto = new RatingDto
            {
                RentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Excelente"
            };

            _ratingServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<RatingDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.StatusCode, Is.EqualTo(200));
            Assert.That(ok.Value, Is.EqualTo("Avaliação criada com sucesso."));

            _ratingServiceMock.Verify(s => s.CreateAsync(It.Is<RatingDto>(x =>
                x.RentId == dto.RentId &&
                x.UserId == dto.UserId &&
                x.ClothingId == dto.ClothingId &&
                x.Rating == dto.Rating &&
                x.Comment == dto.Comment
            )), Times.Once);
        }

        [Test]
        public async Task GetByClothing_ShouldReturnOk_WithRatings()
        {
            // Arrange
            var clothingId = Guid.NewGuid();

            var ratings = new List<RatingDto>
            {
                new RatingDto
                {
                    RentId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ClothingId = clothingId,
                    Rating = 4,
                    Comment = "Bom"
                }
            };

            _ratingServiceMock
                .Setup(s => s.GetByClothingAsync(clothingId))
                .ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetByClothing(clothingId);

            // Assert
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.StatusCode, Is.EqualTo(200));
            Assert.That(ok.Value, Is.EqualTo(ratings));

            _ratingServiceMock.Verify(s => s.GetByClothingAsync(clothingId), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnNoContent_AndCallService()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _ratingServiceMock
                .Setup(s => s.DeleteAsync(ratingId, userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(ratingId, userId);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());

            _ratingServiceMock.Verify(s => s.DeleteAsync(ratingId, userId), Times.Once);
        }
    }
}
