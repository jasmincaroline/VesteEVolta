using VesteEVolta.Models;

namespace VesteEVolta.Repositories;

public interface IClothingRepository
{
    Task<List<TbClothing>> GetAllAsync(Guid? categoryId, string? status, decimal? minPrice, decimal? maxPrice);
    Task<TbClothing?> GetByIdAsync(Guid id);
    Task<List<TbClothing>> GetByOwnerIdAsync(Guid ownerId);
}
