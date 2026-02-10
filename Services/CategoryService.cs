using VesteEVolta.Contracts.DTO;
using VesteEVolta.Contracts.Mappers;
using VesteEVolta.Repositories.Interfaces;
using VesteEVolta.Services.Interfaces;

namespace VesteEVolta.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => c.ToDto());
    }
}
