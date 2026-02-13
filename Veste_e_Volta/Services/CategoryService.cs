using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Repositories;

namespace VesteEVolta.Services
{
    public class CategoryService : ICategoryService
    {   
        private readonly ICategoryRepositories _repositories;

        public CategoryService(ICategoryRepositories repositories)
        {
            _repositories = repositories;
        }

        public List<CategoryResponseDto> GetCategories()
        {
            var categories = _repositories.GetAll();

            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            }).ToList();
        }
    public CategoryResponseDto UpdateCategory(Guid id, CategoryRequestDto dto)
        {
            var category = _repositories.GetById(id);
            if (category == null)
                throw new KeyNotFoundException("Categoria não encontrada :(");

            var existing = _repositories.GetByName(dto.Name);
            if (existing != null && existing.CategoryId != id)
                throw new InvalidOperationException("Já existe uma categoria com esse nome, tente outro :)");

            category.Name = dto.Name.Trim();

            _repositories.Update(category);
            _repositories.SaveChanges();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public void DeleteCategory(Guid id)
        {
            var category = _repositories.GetById(id);
            if (category == null)
                throw new KeyNotFoundException("Categoria não encontrada :(");

            // 409 se tiver roupas vinculadas
            if (category.Clothings != null && category.Clothings.Any())
                throw new InvalidOperationException("Puxa, essa categoria possui roupas vinculadas, não tem como excluir.");

            _repositories.Delete(category);
            _repositories.SaveChanges();
        }
    }
}
