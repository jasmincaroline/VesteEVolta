using VesteEVolta.Models;
namespace VesteEVolta.Repositories
{
    public class CategoryRepositories : ICategoryRepositories
    {
        private readonly PostgresContext _context;

        public CategoryRepositories(PostgresContext context)
        {
            _context = context;
        }

        public List<TbCategory> GetAll()
        {
            return _context.TbCategories.ToList();
        }
    }
}