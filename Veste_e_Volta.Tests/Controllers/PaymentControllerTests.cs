using Moq;
using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Services;
using VesteEVolta.Models;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentServiceMock.Object);
        }

        #region GetByRental Tests

        [Test]
        public async Task GetByRental_ValidRentalId_ReturnsOkWithPayments()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var payments = new List<TbPayment>
            {
                new TbPayment 
                { 
                    Id = Guid.NewGuid(), 
                    RentalId = rentalId,
                    Amount = 100,
                    PaymentMethod = "credit_card",
                    PaymentStatus = "completed",
                    PaymentDate = DateTime.UtcNow
                }
            };

            _paymentServiceMock
                .Setup(s => s.GetByRentalId(rentalId))
                .ReturnsAsync(payments);

            // Act
            var result = await _controller.GetByRental(rentalId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(payments));
            _paymentServiceMock.Verify(s => s.GetByRentalId(rentalId), Times.Once);
        }

        [Test]
        public async Task GetByRental_NoPayments_ReturnsOkWithEmptyList()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var emptyList = new List<TbPayment>();

            _paymentServiceMock
                .Setup(s => s.GetByRentalId(rentalId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetByRental(rentalId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(emptyList));
            _paymentServiceMock.Verify(s => s.GetByRentalId(rentalId), Times.Once);
        }

        [Test]
        public void GetByRental_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            _paymentServiceMock
                .Setup(s => s.GetByRentalId(rentalId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.That(async () => await _controller.GetByRental(rentalId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Database error"));
            _paymentServiceMock.Verify(s => s.GetByRentalId(rentalId), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_PaymentExists_ReturnsOkWithPayment()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var payment = new TbPayment 
            { 
                Id = paymentId,
                Amount = 150,
                PaymentMethod = "pix",
                PaymentStatus = "pending",
                RentalId = Guid.NewGuid(),
                PaymentDate = DateTime.UtcNow
            };

            _paymentServiceMock
                .Setup(s => s.GetById(paymentId))
                .ReturnsAsync(payment);

            // Act
            var result = await _controller.GetById(paymentId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(payment));
            _paymentServiceMock.Verify(s => s.GetById(paymentId), Times.Once);
        }

        [Test]
        public async Task GetById_PaymentNotFound_ReturnsNotFound()
        {
            // Arrange
            var paymentId = Guid.NewGuid();

            _paymentServiceMock
                .Setup(s => s.GetById(paymentId))  
                .ReturnsAsync((TbPayment?)null!);

            // Act
            var result = await _controller.GetById(paymentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            _paymentServiceMock.Verify(s => s.GetById(paymentId), Times.Once);
        }

        [Test]
        public void GetById_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            _paymentServiceMock
                .Setup(s => s.GetById(paymentId))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            Assert.That(async () => await _controller.GetById(paymentId),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Database connection failed"));
            _paymentServiceMock.Verify(s => s.GetById(paymentId), Times.Once);
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreatePaymentDto
            {
                RentalId = Guid.NewGuid(),
                PaymentMethod = "credit_card",
                Amount = 200,
                PaymentStatus = "pending"
            };

            var createdPayment = new TbPayment
            {
                Id = Guid.NewGuid(),
                RentalId = dto.RentalId,
                PaymentMethod = dto.PaymentMethod,
                Amount = dto.Amount,
                PaymentStatus = dto.PaymentStatus,
                PaymentDate = DateTime.UtcNow
            };

            _paymentServiceMock
                .Setup(s => s.Create(dto.RentalId, dto))
                .ReturnsAsync(createdPayment);

            // Act
            var result = await _controller.Create(dto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(PaymentController.GetById)));
            Assert.That(result.Value, Is.EqualTo(createdPayment));
            Assert.That(result.RouteValues?["id"], Is.EqualTo(createdPayment.Id));
            _paymentServiceMock.Verify(s => s.Create(dto.RentalId, dto), Times.Once);
        }

        [Test]
        public async Task Create_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreatePaymentDto
            {
                RentalId = Guid.NewGuid(),
                PaymentMethod = "credit_card",
                Amount = 200
            };

            _paymentServiceMock
                .Setup(s => s.Create(dto.RentalId, dto))
                .ThrowsAsync(new Exception("Rental not found"));

            // Act
            var result = await _controller.Create(dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Rental not found"));
            _paymentServiceMock.Verify(s => s.Create(dto.RentalId, dto), Times.Once);
        }

        [Test]
        public async Task Create_InvalidAmount_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreatePaymentDto
            {
                RentalId = Guid.NewGuid(),
                PaymentMethod = "credit_card",
                Amount = -100
            };

            _paymentServiceMock
                .Setup(s => s.Create(dto.RentalId, dto))
                .ThrowsAsync(new ArgumentException("Amount must be positive"));

            // Act
            var result = await _controller.Create(dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Amount must be positive"));
            _paymentServiceMock.Verify(s => s.Create(dto.RentalId, dto), Times.Once);
        }

        [Test]
        public async Task Create_EmptyPaymentMethod_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreatePaymentDto
            {
                RentalId = Guid.NewGuid(),
                PaymentMethod = "",
                Amount = 100
            };

            _paymentServiceMock
                .Setup(s => s.Create(dto.RentalId, dto))
                .ThrowsAsync(new ArgumentException("Payment method is required"));

            // Act
            var result = await _controller.Create(dto) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _paymentServiceMock.Verify(s => s.Create(dto.RentalId, dto), Times.Once);
        }

        #endregion
    }
}
