using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using VesteEVolta.Controllers;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;

namespace VesteEVolta.Tests.Controllers
{
    [TestFixture]
    public class ReportControllerTests
    {
        private Mock<IReportService> _reportServiceMock = null!;
        private PostgresContext _context = null!;
        private ReportsController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _reportServiceMock = new Mock<IReportService>();

            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostgresContext(options);
            _controller = new ReportsController(_reportServiceMock.Object, _context);

            // Setup user claims for authentication
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Create Tests

        [Test]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var reporterId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dto = new CreateReportDto
            {
                ReporterId = reporterId,
                ReportedId = Guid.NewGuid(),
                ReportedClothingId = Guid.NewGuid(),
                Type = "USER",
                Reason = "Comportamento inadequado"
            };

            var createdReport = new TbReport
            {
                ReportId = Guid.NewGuid(),
                ReporterId = reporterId,
                ReportedId = dto.ReportedId,
                ReportedClothingId = dto.ReportedClothingId,
                Type = dto.Type,
                Reason = dto.Reason,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _reportServiceMock
                .Setup(s => s.CreateAsync(reporterId, dto))
                .ReturnsAsync(createdReport);

            // Act
            var result = await _controller.Create(dto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo(nameof(ReportsController.GetById)));
            Assert.That(result.Value, Is.EqualTo(createdReport));
            _reportServiceMock.Verify(s => s.CreateAsync(reporterId, dto), Times.Once);
        }

        [Test]
        public async Task Create_InvalidUserId_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            var dto = new CreateReportDto
            {
                ReportedId = Guid.NewGuid(),
                ReportedClothingId = Guid.NewGuid(),
                Type = "USER",
                Reason = "Teste"
            };

            // Act
            var result = await _controller.Create(dto) as UnauthorizedObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("Token sem userId válido."));
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsOkWithAllReports()
        {
            // Arrange
            var reports = new List<TbReport>
            {
                new TbReport
                {
                    ReportId = Guid.NewGuid(),
                    ReporterId = Guid.NewGuid(),
                    ReportedId = Guid.NewGuid(),
                    ReportedClothingId = Guid.NewGuid(),
                    Type = "USER",
                    Reason = "Motivo 1",
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                },
                new TbReport
                {
                    ReportId = Guid.NewGuid(),
                    ReporterId = Guid.NewGuid(),
                    ReportedId = Guid.NewGuid(),
                    ReportedClothingId = Guid.NewGuid(),
                    Type = "CLOTHING",
                    Reason = "Motivo 2",
                    Status = "RESOLVED",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _reportServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(reports);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(reports));
            _reportServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAll_NoReports_ReturnsEmptyList()
        {
            // Arrange
            _reportServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<TbReport>());

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var reports = result.Value as List<TbReport>;
            Assert.That(reports, Is.Not.Null);
            Assert.That(reports.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ReportExists_ReturnsOkWithReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new TbReport
            {
                ReportId = reportId,
                ReporterId = Guid.NewGuid(),
                ReportedId = Guid.NewGuid(),
                ReportedClothingId = Guid.NewGuid(),
                Type = "USER",
                Reason = "Teste",
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _reportServiceMock
                .Setup(s => s.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            // Act
            var result = await _controller.GetById(reportId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(report));
            _reportServiceMock.Verify(s => s.GetByIdAsync(reportId), Times.Once);
        }

        [Test]
        public async Task GetById_ReportNotFound_ReturnsNotFound()
        {
            // Arrange
            var reportId = Guid.NewGuid();

            _reportServiceMock
                .Setup(s => s.GetByIdAsync(reportId))
                .ReturnsAsync((TbReport?)null);

            // Act
            var result = await _controller.GetById(reportId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _reportServiceMock.Verify(s => s.GetByIdAsync(reportId), Times.Once);
        }

        #endregion

        #region GetRentalsCsv Tests

        [Test]
        public async Task GetRentalsCsv_ValidDateRange_ReturnsCsvFile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var user = new TbUser
            {
                Id = userId,
                Name = "João Silva",
                Email = "joao@test.com",
                PasswordHash = "hash",
                Telephone = "11999999999",
                ProfileType = "customer",
                CreatedAt = DateTime.UtcNow
            };

            var owner = new TbOwner { UserId = ownerId };

            var clothing = new TbClothing
            {
                Id = Guid.NewGuid(),
                Description = "Vestido elegante",
                RentPrice = 100,
                AvailabilityStatus = "RENTED",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            var rental = new TbRental
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClothingId = clothing.Id,
                StartDate = new DateOnly(2026, 2, 10),
                EndDate = new DateOnly(2026, 2, 15),
                Status = "ACTIVE",
                TotalValue = 500,
                CreatedAt = DateTime.UtcNow
            };

            await _context.TbUsers.AddAsync(user);
            await _context.TbOwners.AddAsync(owner);
            await _context.TbClothings.AddAsync(clothing);
            await _context.TbRentals.AddAsync(rental);
            await _context.SaveChangesAsync();

            var from = new DateOnly(2026, 2, 1);
            var to = new DateOnly(2026, 2, 28);

            // Act
            var result = await _controller.GetRentalsCsv(from, to) as FileContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
            Assert.That(result.FileDownloadName, Is.EqualTo("rentals_20260201_20260228.csv"));

            var csvContent = System.Text.Encoding.UTF8.GetString(result.FileContents);
            Assert.That(csvContent, Does.Contain("RentalId,UserId,UserName,ClothingId,Description,StartDate,EndDate,Status,TotalValue"));
            Assert.That(csvContent, Does.Contain("João Silva"));
            Assert.That(csvContent, Does.Contain("Vestido elegante"));
        }

        [Test]
        public async Task GetRentalsCsv_InvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var from = new DateOnly(2026, 2, 28);
            var to = new DateOnly(2026, 2, 1);

            // Act
            var result = await _controller.GetRentalsCsv(from, to) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo("'from' não pode ser maior que 'to'."));
        }

        [Test]
        public async Task GetRentalsCsv_NoRentalsInRange_ReturnsEmptyCsv()
        {
            // Arrange
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 1, 31);

            // Act
            var result = await _controller.GetRentalsCsv(from, to) as FileContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var csvContent = System.Text.Encoding.UTF8.GetString(result.FileContents);
            Assert.That(csvContent, Does.Contain("RentalId,UserId,UserName,ClothingId,Description,StartDate,EndDate,Status,TotalValue"));
            
            // Should only have header line
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.That(lines.Length, Is.EqualTo(1));
        }

        #endregion

        #region UpdateStatus Tests

        [Test]
        public async Task UpdateStatus_ValidData_ReturnsOkWithUpdatedReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var dto = new UpdateReportStatusDto
            {
                Status = "RESOLVED"
            };

            var updatedReport = new TbReport
            {
                ReportId = reportId,
                ReporterId = Guid.NewGuid(),
                ReportedId = Guid.NewGuid(),
                ReportedClothingId = Guid.NewGuid(),
                Type = "USER",
                Reason = "Teste",
                Status = "RESOLVED",
                CreatedAt = DateTime.UtcNow
            };

            _reportServiceMock
                .Setup(s => s.UpdateStatusAsync(reportId, dto.Status))
                .ReturnsAsync(updatedReport);

            // Act
            var result = await _controller.UpdateStatus(reportId, dto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(updatedReport));
            _reportServiceMock.Verify(s => s.UpdateStatusAsync(reportId, dto.Status), Times.Once);
        }

        [Test]
        public void UpdateStatus_ServiceThrowsException_ExceptionPropagates()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var dto = new UpdateReportStatusDto
            {
                Status = "RESOLVED"
            };

            _reportServiceMock
                .Setup(s => s.UpdateStatusAsync(reportId, dto.Status))
                .ThrowsAsync(new Exception("Report not found"));

            // Act & Assert
            Assert.That(async () => await _controller.UpdateStatus(reportId, dto),
                Throws.Exception.TypeOf<Exception>().With.Message.EqualTo("Report not found"));
            _reportServiceMock.Verify(s => s.UpdateStatusAsync(reportId, dto.Status), Times.Once);
        }

        #endregion
    }
}
