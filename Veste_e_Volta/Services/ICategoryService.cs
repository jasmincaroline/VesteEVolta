using AulaApi.Veste_e_Volta.DTO;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Services
{
    public interface ICategoryService
    {
        List<CategoryResponseDto> GetCategories();
        CategoryResponseDto UpdateCategory(Guid id, CategoryRequestDto dto);
        void DeleteCategory(Guid id);
    }
}
