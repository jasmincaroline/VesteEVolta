using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Services
{
    public interface IReportService
    {
        Task<TbReport> CreateAsync(Guid reporterId, CreateReportDto dto);
        Task<List<TbReport>> GetAllAsync();
        Task<TbReport?> GetByIdAsync(Guid id);
        Task<TbReport> UpdateStatusAsync(Guid id, string status);
    }
}
