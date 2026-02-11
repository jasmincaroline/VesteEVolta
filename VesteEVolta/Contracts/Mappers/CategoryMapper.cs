using VesteEVolta.Contracts.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Contracts.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this TbCategory model)
    {
        return new CategoryDto
        {
            CategoryId = model.CategoryId,
            Name = model.Name
        };
    }

    public static TbCategory ToModel(this CategoryDto dto)
    {
        return new TbCategory
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name
        };
    }
}
