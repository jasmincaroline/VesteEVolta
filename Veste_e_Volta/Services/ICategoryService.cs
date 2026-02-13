using System.Collections.Generic;
using VesteEVolta.DTO;

namespace VesteEVolta.Services
{
    public interface ICategoryService
    {
        List<CategoryResponseDto> GetCategories();
        CategoryResponseDto UpdateCategory(Guid id, CategoryRequestDto dto);
        void DeleteCategory(Guid id);
    }
}