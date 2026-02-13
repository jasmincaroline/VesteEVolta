using VesteEVolta.Models;

namespace VesteEVolta.Repositories;

public class CategoryRepositories : ICategoryRepositories
{
    private readonly PostgresContext _context;

    public CategoryRepositories(PostgresContext context)
    {
        _context = context;
    }

    public List<TbCategory> GetAll()
        => _context.TbCategories.ToList();

    public TbCategory? GetById(Guid id)
        => _context.TbCategories.FirstOrDefault(c => c.CategoryId == id);

    public TbCategory? GetByName(string name)
        => _context.TbCategories.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());

    public void Add(TbCategory category) => _context.TbCategories.Add(category);

    public void Update(TbCategory category) => _context.TbCategories.Update(category);

    public void Delete(TbCategory category) => _context.TbCategories.Remove(category);

    public void SaveChanges() => _context.SaveChanges();
}