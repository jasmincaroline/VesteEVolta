using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using VesteEVolta.DTO;
using VesteEVolta.Exceptions;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Services;

namespace Veste_e_Volta.Tests.Services
{
    [TestFixture]
    public class ReportServiceTests
    {
        private Mock<IReportRepository> _repoMock = null!;
        private ReportService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IReportRepository>();
            _service = new ReportService(_repoMock.Object);
        }

        [Test]
        public void CreateAsync_WhenTypeEmpty_ShouldThrowBusinessException()
        {
            var dto = new CreateReportDto
            {
                Type = "   ",
                Reason = "Motivo",
                ReportedId = Guid.NewGuid()
            };

            Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(Guid.NewGuid(), dto));
        }

        [Test]
        public void CreateAsync_WhenReasonEmpty_ShouldThrowBusinessException()
        {
            var dto = new CreateReportDto
            {
                Type = "USER",
                Reason = "   ",
                ReportedId = Guid.NewGuid()
            };

            Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(Guid.NewGuid(), dto));
        }

        [Test]
        public async Task CreateAsync_WhenValid_ShouldCallRepoCreate_AndReturnCreated()
        {
            var reporterId = Guid.NewGuid();
            var reportedId = Guid.NewGuid();

            var dto = new CreateReportDto
            {
                Type = "USER",
                Reason = "Comportamento inadequado",
                Description = "Detalhes",
                ReportedId = reportedId,
                ReportedClothingId = Guid.NewGuid(),
                RentalId = null
            };

            _repoMock
                .Setup(r => r.CreateAsync(It.IsAny<TbReport>()))
                .ReturnsAsync((TbReport report) => report);

            var created = await _service.CreateAsync(reporterId, dto);

            Assert.That(created, Is.Not.Null);
            Assert.That(created.ReportId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(created.ReporterId, Is.EqualTo(reporterId));
            Assert.That(created.ReportedId, Is.EqualTo(reportedId));
            Assert.That(created.Type, Is.EqualTo(dto.Type));
            Assert.That(created.Reason, Is.EqualTo(dto.Reason));
            Assert.That(created.Status, Is.EqualTo("OPEN"));

            _repoMock.Verify(r => r.CreateAsync(It.IsAny<TbReport>()), Times.Once);
        }

        [Test]
        public void UpdateStatusAsync_WhenStatusInvalid_ShouldThrowBusinessException()
        {
            Assert.ThrowsAsync<BusinessException>(() => _service.UpdateStatusAsync(Guid.NewGuid(), "INVALID"));
        }

        [Test]
        public void UpdateStatusAsync_WhenReportNotFound_ShouldThrowBusinessException()
        {
            _repoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TbReport?)null);

            Assert.ThrowsAsync<BusinessException>(() => _service.UpdateStatusAsync(Guid.NewGuid(), "OPEN"));
        }

        [Test]
        public async Task UpdateStatusAsync_WhenValid_ShouldUpdateStatus_AndSaveChanges()
        {
            var id = Guid.NewGuid();

            var report = new TbReport
            {
                ReportId = id,
                Status = "OPEN"
            };

            _repoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(report);

            var updated = await _service.UpdateStatusAsync(id, "RESOLVED");

            Assert.That(updated.Status, Is.EqualTo("RESOLVED"));
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateAsync_WhenReportedIdEmpty_ShouldThrowBusinessException()
        {
            var dto = new CreateReportDto
            {
                Type = "USER",
                Reason = "Motivo",
                ReportedId = Guid.Empty,
                ReportedClothingId = Guid.NewGuid(),
                RentalId= null
            };
            Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(Guid.NewGuid(), dto));
        }
    }
}
