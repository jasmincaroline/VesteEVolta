using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

namespace VesteEVolta.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly PostgresContext _context;

    public RentalRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<TbRental> Add(TbRental rental)
    {
        _context.TbRentals.Add(rental);
        await _context.SaveChangesAsync();
        return rental;
    }

    public async Task<TbRental?> GetById(Guid id)
    {
        return await _context.TbRentals
            .Include(r => r.User)
            .Include(r => r.Clothing)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<TbRental>> GetAll()
    {
        return await _context.TbRentals
            .Include(r => r.User)
            .Include(r => r.Clothing)
            .ToListAsync();
    }

    public async Task<IEnumerable<TbRental>> GetByUserId(Guid userId)
    {
        return await _context.TbRentals
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TbRental>> GetByClothingId(Guid clothingId)
    {
        return await _context.TbRentals
            .Where(r => r.ClothingId == clothingId)
            .ToListAsync();
    }

    public async Task Update(TbRental rental)
    {
        _context.TbRentals.Update(rental);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var rental = await _context.TbRentals.FindAsync(id);
        if (rental != null)
        {
            _context.TbRentals.Remove(rental);
            await _context.SaveChangesAsync();
        }
    }
}
