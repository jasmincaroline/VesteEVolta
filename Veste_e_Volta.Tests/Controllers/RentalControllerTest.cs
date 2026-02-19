using Moq;
using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Services;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class RentalControllerTests
    {
        private Mock<IRentalService> _rentalServiceMock;
        private RentalController _controller;

        [SetUp]
        public void Setup()
        {
            _rentalServiceMock = new Mock<IRentalService>();
            _controller = new RentalController(_rentalServiceMock.Object);
        }

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedRental()
        {
            // Arrange
            var dto = new RentalDTO { UserId = Guid.NewGuid(), ClothingId = Guid.NewGuid(), Status = "Pending" };
            var response = new RentalResponseDTO { Id = Guid.NewGuid(), UserId = dto.UserId, ClothingId = dto.ClothingId, Status = dto.Status };

            _rentalServiceMock
                .Setup(s => s.Create(It.Is<RentalDTO>(d => d == dto)))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Create(dto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(RentalController.GetById)));
            Assert.That(result.Value, Is.EqualTo(response));
            _rentalServiceMock.Verify(s => s.Create(dto), Times.Once);
        }

        [Test]
        public void Create_ServiceThrows_ExceptionPropagates()
        {
            // Arrange
            var dto = new RentalDTO();
            _rentalServiceMock
                .Setup(s => s.Create(It.IsAny<RentalDTO>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            Assert.That(async () => await _controller.Create(dto),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Service error"));
            _rentalServiceMock.Verify(s => s.Create(dto), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_RentalsExist_ReturnsOkWithRentals()
        {
            // Arrange
            var rentals = new List<RentalResponseDTO>
            {
                new RentalResponseDTO { Id = Guid.NewGuid(), Status = "Pending" }
            };

            _rentalServiceMock.Setup(s => s.GetAll()).ReturnsAsync(rentals);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(rentals));
            _rentalServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_RentalExists_ReturnsOkWithRental()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var response = new RentalResponseDTO { Id = rentalId, Status = "Pending" };

            _rentalServiceMock.Setup(s => s.GetById(rentalId)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(rentalId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(response));
            _rentalServiceMock.Verify(s => s.GetById(rentalId), Times.Once);
        }

        [Test]
        public async Task GetById_RentalDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var rentalId = Guid.NewGuid();

            _rentalServiceMock
            .Setup(s => s.GetById(rentalId))
            .ReturnsAsync((RentalResponseDTO?)null!);

            // Act
            var result = await _controller.GetById(rentalId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            _rentalServiceMock.Verify(s => s.GetById(rentalId), Times.Once);
        }

        #endregion

        #region UpdateStatus Tests

        [Test]
        public async Task UpdateStatus_ValidStatus_ReturnsNoContent()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var status = "Approved";

            _rentalServiceMock.Setup(s => s.UpdateStatus(rentalId, status)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStatus(rentalId, status);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _rentalServiceMock.Verify(s => s.UpdateStatus(rentalId, status), Times.Once);
        }

        [Test]
        public void UpdateStatus_ServiceThrows_ExceptionPropagates()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var status = "Approved";
            _rentalServiceMock.Setup(s => s.UpdateStatus(rentalId, status))
                .ThrowsAsync(new Exception("Service error"));

            // Act & Assert
            Assert.That(async () => await _controller.UpdateStatus(rentalId, status),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Service error"));
            _rentalServiceMock.Verify(s => s.UpdateStatus(rentalId, status), Times.Once);
        }

        #endregion

        #region GetByUser Tests

        [Test]
        public async Task GetByUser_RentalsExist_ReturnsOkWithRentals()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var rentals = new List<RentalResponseDTO>
            {
                new RentalResponseDTO { Id = Guid.NewGuid(), Status = "Pending" }
            };

            _rentalServiceMock.Setup(s => s.GetByUserId(userId)).ReturnsAsync(rentals);

            // Act
            var result = await _controller.GetByUser(userId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(rentals));
            _rentalServiceMock.Verify(s => s.GetByUserId(userId), Times.Once);
        }

        #endregion

        #region GetByClothing Tests

        [Test]
        public async Task GetByClothing_RentalsExist_ReturnsOkWithRentals()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var rentals = new List<RentalResponseDTO>
            {
                new RentalResponseDTO { Id = Guid.NewGuid(), Status = "Pending" }
            };

            _rentalServiceMock.Setup(s => s.GetByClothingId(clothingId)).ReturnsAsync(rentals);

            // Act
            var result = await _controller.GetByClothing(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(rentals));
            _rentalServiceMock.Verify(s => s.GetByClothingId(clothingId), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var deletedRental = new RentalResponseDTO 
            { 
                Id = rentalId, 
                Status = "active",
                TotalValue = 100 
            };

            _rentalServiceMock
                .Setup(s => s.Delete(rentalId))
                .ReturnsAsync(deletedRental);

            // Act
            var result = await _controller.Delete(rentalId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _rentalServiceMock.Verify(s => s.Delete(rentalId), Times.Once);
        }

        [Test]
        public void Delete_RentalNotFound_ExceptionPropagates()
        {
            // Arrange
            var rentalId = Guid.NewGuid();

            _rentalServiceMock
                .Setup(s => s.Delete(rentalId))
                .ThrowsAsync(new Exception("Aluguel não encontrado"));

            // Act & Assert
            Assert.That(async () => await _controller.Delete(rentalId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Aluguel não encontrado"));
            _rentalServiceMock.Verify(s => s.Delete(rentalId), Times.Once);
        }

        [Test]
        public void Delete_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var rentalId = Guid.NewGuid();

            _rentalServiceMock
                .Setup(s => s.Delete(rentalId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.That(async () => await _controller.Delete(rentalId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Database error"));
            _rentalServiceMock.Verify(s => s.Delete(rentalId), Times.Once);
        }

        #endregion

        #region Additional Edge Cases

        [Test]
        public async Task Create_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RentalDTO();
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            // Act
            var result = await _controller.Create(dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _rentalServiceMock.Verify(s => s.Create(It.IsAny<RentalDTO>()), Times.Never);
        }

        [Test]
        public async Task GetAll_NoRentals_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<RentalResponseDTO>();
            _rentalServiceMock.Setup(s => s.GetAll()).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _rentalServiceMock.Verify(s => s.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetByUser_NoRentals_ReturnsOkWithEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyList = new List<RentalResponseDTO>();
            _rentalServiceMock.Setup(s => s.GetByUserId(userId)).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetByUser(userId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _rentalServiceMock.Verify(s => s.GetByUserId(userId), Times.Once);
        }

        [Test]
        public async Task GetByClothing_NoRentals_ReturnsOkWithEmptyList()
        {
            // Arrange
            var clothingId = Guid.NewGuid();
            var emptyList = new List<RentalResponseDTO>();
            _rentalServiceMock.Setup(s => s.GetByClothingId(clothingId)).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetByClothing(clothingId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _rentalServiceMock.Verify(s => s.GetByClothingId(clothingId), Times.Once);
        }

        #endregion

    }
}
