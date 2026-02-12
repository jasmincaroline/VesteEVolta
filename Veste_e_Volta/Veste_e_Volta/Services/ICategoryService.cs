using VesteEVolta.Models;
using VesteEVolta.DTO;

namespace VesteEVolta.Services
{
    public interface ICategoryService
    {
        List<CategoryResponseDto> GetCategories();
    }
}
