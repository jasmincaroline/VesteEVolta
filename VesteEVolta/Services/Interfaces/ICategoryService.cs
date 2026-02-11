using VesteEVolta.Contracts.DTO;

namespace VesteEVolta.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(CategoryDto categoryDto);
}
