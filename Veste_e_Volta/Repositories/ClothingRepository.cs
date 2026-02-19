using System;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;


namespace VesteEVolta.Repositories;

public class ClothingRepository : IClothingRepository
{
    private readonly PostgresContext _context;

    public ClothingRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<TbClothing>> GetAllAsync(Guid? categoryId, string? status, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.TbClothings
            .Include(c => c.Categories)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(c => c.Categories.Any(cat => categoryId == categoryId.Value));

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.AvailabilityStatus == status);

        if (minPrice.HasValue)
            query = query.Where(c => c.RentPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.RentPrice <= maxPrice.Value);

        return await query.ToListAsync();
    }

    public async Task<TbClothing?> GetByIdAsync(Guid id)
    {
        return await _context.TbClothings
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<TbClothing>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.TbClothings
            .Where(c => c.OwnerId == ownerId)
            .Include(c => c.Categories)
            .ToListAsync();
    }
}
