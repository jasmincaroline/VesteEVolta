using VesteEVolta.Contracts.DTO;

namespace VesteEVolta.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
}
