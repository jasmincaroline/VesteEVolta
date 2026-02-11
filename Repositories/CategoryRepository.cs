using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
using VesteEVolta.Repositories.Interfaces;

namespace VesteEVolta.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly VesteEVoltaContext _context;

    public CategoryRepository(VesteEVoltaContext context)
    {
        _context = context;
    }

    public async Task<List<TbCategory>> GetAllAsync()
    {
        return await _context.TbCategories
            .AsNoTracking()
            .ToListAsync();
    }
}
