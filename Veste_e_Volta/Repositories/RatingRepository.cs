using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

public class RatingRepository : IRatingRepository
{
    private readonly PostgresContext _context;

    public RatingRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TbRating rating)
    {
        await _context.TbRatings.AddAsync(rating);
        await _context.SaveChangesAsync();
    }

    public async Task<TbRating?> GetByIdAsync(Guid id)
    {
        return await _context.TbRatings.FindAsync(id);
    }

    public async Task<TbRating?> GetByRentIdAsync(Guid rentId)
    {
        return await _context.TbRatings
            .FirstOrDefaultAsync(r => r.RentId == rentId);
    }

    public async Task<List<TbRating>> GetByClothingIdAsync(Guid clothingId)
    {
        return await _context.TbRatings
            .Where(r => r.ClothingId == clothingId)
            .ToListAsync();
    }

    public async Task<List<TbRating>> GetByUserIdAsync(Guid userId)
    {
        return await _context.TbRatings
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task DeleteAsync(TbRating rating)
    {
        _context.TbRatings.Remove(rating);
        await _context.SaveChangesAsync();
    }
}
