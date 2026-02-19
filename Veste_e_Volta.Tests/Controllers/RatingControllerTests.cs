using Moq;
using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Controllers;
using VesteEVolta.Application.DTOs;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class RatingControllerTests
    {
        private Mock<IRatingService> _ratingServiceMock;
        private RatingController _controller;

        [SetUp]
        public void Setup()
        {
            _ratingServiceMock = new Mock<IRatingService>();
            _controller = new RatingController(_ratingServiceMock.Object);
        }

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            var dto = new RatingDto
            {
                UserId = Guid.NewGuid(),
                RentId = Guid.NewGuid(),
                ClothingId = Guid.NewGuid(),
                Rating = 5,
                Comment = "Excelente produto!",
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _ratingServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<RatingDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Avaliação criada com sucesso."));
            _ratingServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Test]
        public void Create_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var dto = new RatingDto
            {
                UserId = Guid.NewGuid(),
                Rating = 5
            };

            _ratingServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<RatingDto>()))
                .ThrowsAsync(new Exception("Rental not found"));

            // Act & Assert
            Assert.That(async () => await _controller.Create(dto),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Rental not found"));
            _ratingServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Test]
        public void Create_InvalidRating_ExceptionPropagates()
        {
            // Arrange
            var dto = new RatingDto
            {
                UserId = Guid.NewGuid(),
                Rating = 6 // Invalid rating (should be 1-5)
            };

            _ratingServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<RatingDto>()))
                .ThrowsAsync(new ArgumentException("Rating must be between 1 and 5"));

            // Act & Assert
            Assert.That(async () => await _controller.Create(dto),
                Throws.ArgumentException.With.Message.EqualTo("Rating must be between 1 and 5"));
            _ratingServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_RatingsExist_ReturnsOkWithRatings()
        {
            // Arrange
            var ratings = new List<RatingDto>
            {
                new RatingDto 
                { 
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ClothingId = Guid.NewGuid(),
                    Rating = 5,
                    Comment = "Great!"
                },
                new RatingDto 
                { 
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ClothingId = Guid.NewGuid(),
                    Rating = 4,
                    Comment = "Good"
                }
            };

            _ratingServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(ratings));
            _ratingServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAll_NoRatings_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<RatingDto>();

            _ratingServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _ratingServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Test]
        public void GetAll_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            _ratingServiceMock
                .Setup(s => s.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.That(async () => await _controller.GetAll(),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Database error"));
            _ratingServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        #endregion

        #region GetByClothing Tests

        [Test]
        public async Task GetByClothing_RatingsExist_ReturnsOkWithRatings()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var ratings = new List<RatingDto>
            {
                new RatingDto 
                { 
                    Id = Guid.NewGuid(),
                    ClothingId = clothingId,
                    Rating = 5,
                    Comment = "Excellent"
                }
            };

            _ratingServiceMock
                .Setup(s => s.GetByClothingAsync(clothingId))
                .ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetByClothing(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(ratings));
            _ratingServiceMock.Verify(s => s.GetByClothingAsync(clothingId), Times.Once);
        }

        [Test]
        public async Task GetByClothing_NoRatings_ReturnsOkWithEmptyList()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var emptyList = new List<RatingDto>();

            _ratingServiceMock
                .Setup(s => s.GetByClothingAsync(clothingId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetByClothing(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _ratingServiceMock.Verify(s => s.GetByClothingAsync(clothingId), Times.Once);
        }

        [Test]
        public void GetByClothing_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            _ratingServiceMock
                .Setup(s => s.GetByClothingAsync(clothingId))
                .ThrowsAsync(new Exception("Clothing not found"));

            // Act & Assert
            Assert.That(async () => await _controller.GetByClothing(clothingId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Clothing not found"));
            _ratingServiceMock.Verify(s => s.GetByClothingAsync(clothingId), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ValidIds_ReturnsNoContent()
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
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _ratingServiceMock.Verify(s => s.DeleteAsync(ratingId, userId), Times.Once);
        }

        [Test]
        public void Delete_RatingNotFound_ExceptionPropagates()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _ratingServiceMock
                .Setup(s => s.DeleteAsync(ratingId, userId))
                .ThrowsAsync(new Exception("Rating not found"));

            // Act & Assert
            Assert.That(async () => await _controller.Delete(ratingId, userId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Rating not found"));
            _ratingServiceMock.Verify(s => s.DeleteAsync(ratingId, userId), Times.Once);
        }

        [Test]
        public void Delete_UnauthorizedUser_ExceptionPropagates()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _ratingServiceMock
                .Setup(s => s.DeleteAsync(ratingId, userId))
                .ThrowsAsync(new UnauthorizedAccessException("User not authorized to delete this rating"));

            // Act & Assert
            Assert.That(async () => await _controller.Delete(ratingId, userId),
                Throws.Exception.TypeOf<UnauthorizedAccessException>());
            _ratingServiceMock.Verify(s => s.DeleteAsync(ratingId, userId), Times.Once);
        }

        #endregion

        #region GenerateReport Tests

        [Test]
        public async Task GenerateReport_ReportExists_ReturnsFileWithCsv()
        {
            // Arrange
            var csvContent = System.Text.Encoding.UTF8.GetBytes("Id,Rating,Comment\n1,5,Great");

            _ratingServiceMock
                .Setup(s => s.GenerateReportAsync())
                .ReturnsAsync(csvContent);

            // Act
            var result = await _controller.GenerateReport() as FileContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
            Assert.That(result.FileDownloadName, Is.EqualTo("rating-report.csv"));
            Assert.That(result.FileContents.Length, Is.GreaterThan(0));
            _ratingServiceMock.Verify(s => s.GenerateReportAsync(), Times.Once);
        }

        [Test]
        public void GenerateReport_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            _ratingServiceMock
                .Setup(s => s.GenerateReportAsync())
                .ThrowsAsync(new Exception("Failed to generate report"));

            // Act & Assert
            Assert.That(async () => await _controller.GenerateReport(),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Failed to generate report"));
            _ratingServiceMock.Verify(s => s.GenerateReportAsync(), Times.Once);
        }

        #endregion

        #region GeneratePdfReport Tests

        [Test]
        public async Task GeneratePdfReport_ReportExists_ReturnsFileWithPdf()
        {
            // Arrange
            var pdfContent = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF header

            _ratingServiceMock
                .Setup(s => s.GeneratePdfReportAsync())
                .ReturnsAsync(pdfContent);

            // Act
            var result = await _controller.GeneratePdfReport() as FileContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo("application/pdf"));
            Assert.That(result.FileDownloadName, Is.EqualTo("rating-report.pdf"));
            Assert.That(result.FileContents, Is.EqualTo(pdfContent));
            _ratingServiceMock.Verify(s => s.GeneratePdfReportAsync(), Times.Once);
        }

        [Test]
        public void GeneratePdfReport_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            _ratingServiceMock
                .Setup(s => s.GeneratePdfReportAsync())
                .ThrowsAsync(new Exception("PDF generation failed"));

            // Act & Assert
            Assert.That(async () => await _controller.GeneratePdfReport(),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("PDF generation failed"));
            _ratingServiceMock.Verify(s => s.GeneratePdfReportAsync(), Times.Once);
        }

        #endregion
    }
}
