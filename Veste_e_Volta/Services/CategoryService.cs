using VesteEVolta.DTO;
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
    }
}