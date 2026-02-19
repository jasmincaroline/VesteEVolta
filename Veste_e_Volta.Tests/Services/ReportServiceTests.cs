using Moq;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Services;

[TestFixture]
public class ReportServiceTests
{
    private Mock<IReportRepository> _reportRepositoryMock;
    private ReportService _service;

    [SetUp]
    public void Setup()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _service = new ReportService(_reportRepositoryMock.Object);
    }

    #region CreateAsync Tests

    [Test]
    public async Task CreateAsync_ValidData_ReturnsCreatedReport()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            RentalId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "Inappropriate conduct",
            Description = "User was rude during exchange"
        };

        var expectedReport = new TbReport
        {
            ReportId = Guid.NewGuid(),
            ReporterId = reporterId,
            ReportedId = dto.ReportedId,
            ReportedClothingId = dto.ReportedClothingId,
            RentalId = dto.RentalId,
            Type = dto.Type,
            Reason = dto.Reason,
            Description = dto.Description,
            Status = "OPEN",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        _reportRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TbReport>()))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.CreateAsync(reporterId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReporterId, Is.EqualTo(reporterId));
        Assert.That(result.ReportedId, Is.EqualTo(dto.ReportedId));
        Assert.That(result.Type, Is.EqualTo(dto.Type));
        Assert.That(result.Reason, Is.EqualTo(dto.Reason));
        Assert.That(result.Status, Is.EqualTo("OPEN"));

        _reportRepositoryMock.Verify(
            r => r.CreateAsync(It.Is<TbReport>(report =>
                report.ReporterId == reporterId &&
                report.ReportedId == dto.ReportedId &&
                report.Type == dto.Type &&
                report.Reason == dto.Reason &&
                report.Status == "OPEN")),
            Times.Once);
    }

    [Test]
    public void CreateAsync_EmptyType_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "",
            Reason = "Some reason"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Type é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public void CreateAsync_NullType_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = null!,
            Reason = "Some reason"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Type é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public void CreateAsync_WhitespaceType_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "   ",
            Reason = "Some reason"
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Type é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public void CreateAsync_EmptyReason_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = ""
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Reason é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public void CreateAsync_NullReason_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = null!
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Reason é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public void CreateAsync_WhitespaceReason_ThrowsException()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "   "
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.CreateAsync(reporterId, dto));

        Assert.That(ex.Message, Is.EqualTo("Reason é obrigatório."));
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_NullDescription_CreatesReportSuccessfully()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "Some reason",
            Description = null
        };

        var expectedReport = new TbReport
        {
            ReportId = Guid.NewGuid(),
            ReporterId = reporterId,
            Type = dto.Type,
            Reason = dto.Reason,
            Description = null,
            Status = "OPEN"
        };

        _reportRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TbReport>()))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.CreateAsync(reporterId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.Null);
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_NullRentalId_CreatesReportSuccessfully()
    {
        // Arrange
        var reporterId = Guid.NewGuid();
        var dto = new CreateReportDto
        {
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "Some reason",
            RentalId = null
        };

        var expectedReport = new TbReport
        {
            ReportId = Guid.NewGuid(),
            ReporterId = reporterId,
            Type = dto.Type,
            Reason = dto.Reason,
            RentalId = null,
            Status = "OPEN"
        };

        _reportRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TbReport>()))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.CreateAsync(reporterId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.RentalId, Is.Null);
        _reportRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_ReportsExist_ReturnsListOfReports()
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
                Type = "Behavior",
                Reason = "Rude",
                Status = "OPEN",
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            },
            new TbReport
            {
                ReportId = Guid.NewGuid(),
                ReporterId = Guid.NewGuid(),
                ReportedId = Guid.NewGuid(),
                ReportedClothingId = Guid.NewGuid(),
                Type = "Product",
                Reason = "Damaged",
                Status = "IN_PROGRESS",
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            }
        };

        _reportRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(reports);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        _reportRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_NoReports_ReturnsEmptyList()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TbReport>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
        _reportRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsReport()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var expectedReport = new TbReport
        {
            ReportId = reportId,
            ReporterId = Guid.NewGuid(),
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "Rude",
            Status = "OPEN",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.GetByIdAsync(reportId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReportId, Is.EqualTo(reportId));
        _reportRepositoryMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var reportId = Guid.NewGuid();

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync((TbReport?)null);

        // Act
        var result = await _service.GetByIdAsync(reportId);

        // Assert
        Assert.That(result, Is.Null);
        _reportRepositoryMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
    }

    #endregion

    #region UpdateStatusAsync Tests

    [Test]
    public async Task UpdateStatusAsync_ValidStatusOpenToInProgress_UpdatesSuccessfully()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new TbReport
        {
            ReportId = reportId,
            ReporterId = Guid.NewGuid(),
            ReportedId = Guid.NewGuid(),
            ReportedClothingId = Guid.NewGuid(),
            Type = "Behavior",
            Reason = "Rude",
            Status = "OPEN",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow
        };

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync(report);

        _reportRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateStatusAsync(reportId, "IN_PROGRESS");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo("IN_PROGRESS"));
        _reportRepositoryMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateStatusAsync_ValidStatusToResolved_UpdatesSuccessfully()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new TbReport
        {
            ReportId = reportId,
            Status = "IN_PROGRESS"
        };

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync(report);

        _reportRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateStatusAsync(reportId, "RESOLVED");

        // Assert
        Assert.That(result.Status, Is.EqualTo("RESOLVED"));
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateStatusAsync_ValidStatusToRejected_UpdatesSuccessfully()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var report = new TbReport
        {
            ReportId = reportId,
            Status = "OPEN"
        };

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync(report);

        _reportRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateStatusAsync(reportId, "REJECTED");

        // Assert
        Assert.That(result.Status, Is.EqualTo("REJECTED"));
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void UpdateStatusAsync_InvalidStatus_ThrowsException()
    {
        // Arrange
        var reportId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.UpdateStatusAsync(reportId, "INVALID_STATUS"));

        Assert.That(ex.Message, Is.EqualTo("Status inválido."));
        _reportRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public void UpdateStatusAsync_EmptyStatus_ThrowsException()
    {
        // Arrange
        var reportId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.UpdateStatusAsync(reportId, ""));

        Assert.That(ex.Message, Is.EqualTo("Status inválido."));
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public void UpdateStatusAsync_LowercaseStatus_ThrowsException()
    {
        // Arrange
        var reportId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.UpdateStatusAsync(reportId, "open"));

        Assert.That(ex.Message, Is.EqualTo("Status inválido."));
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public void UpdateStatusAsync_NonExistingReport_ThrowsException()
    {
        // Arrange
        var reportId = Guid.NewGuid();

        _reportRepositoryMock
            .Setup(r => r.GetByIdAsync(reportId))
            .ReturnsAsync((TbReport?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _service.UpdateStatusAsync(reportId, "RESOLVED"));

        Assert.That(ex.Message, Is.EqualTo("Report não encontrado."));
        _reportRepositoryMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
        _reportRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    #endregion
}
