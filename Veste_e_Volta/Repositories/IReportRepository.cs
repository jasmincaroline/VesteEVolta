using VesteEVolta.Models;

namespace VesteEVolta.Repositories
{
    public interface IReportRepository
    {
        Task<TbReport> CreateAsync(TbReport report);
        Task<List<TbReport>> GetAllAsync();
        Task<TbReport?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
