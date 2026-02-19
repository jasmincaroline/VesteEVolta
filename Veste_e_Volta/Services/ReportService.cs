using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;
using VesteEVolta.Exceptions;

namespace VesteEVolta.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        private static readonly HashSet<string> AllowedStatuses =
            new() { "OPEN", "IN_PROGRESS", "RESOLVED", "REJECTED" };

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<TbReport> CreateAsync(Guid reporterId, CreateReportDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Type))
                throw new BusinessException("O tipo do report é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                throw new BusinessException("O motivo do report é obrigatório.");

            if (dto.ReportedId == Guid.Empty)
                throw new BusinessException("O usuário reportado é obrigatório.");

            if (dto.ReportedClothingId == Guid.Empty)
                throw new BusinessException("A roupa reportada é obrigatória.");

            var report = new TbReport
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

            return await _repo.CreateAsync(report);
        }

        public Task<List<TbReport>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<TbReport?> GetByIdAsync(Guid id)
            => _repo.GetByIdAsync(id);

        public async Task<TbReport> UpdateStatusAsync(Guid id, string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new BusinessException("Status inválido.");

            var report = await _repo.GetByIdAsync(id);
            if (report is null)
                throw new BusinessException("Report não encontrado.");

            report.Status = status;
            await _repo.SaveChangesAsync();

            return report;
        }
    }
}
