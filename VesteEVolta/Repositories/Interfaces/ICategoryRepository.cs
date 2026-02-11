using VesteEVolta.Models;

namespace VesteEVolta.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<TbCategory>> GetAllAsync();
    Task<TbCategory?> GetByIdAsync(Guid id);
    Task UpdateAsync(TbCategory category);
}
