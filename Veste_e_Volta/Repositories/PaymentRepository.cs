using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
public class PaymentRepository : IPaymentRepository
{
    private readonly PostgresContext _context;

    public PaymentRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<TbPayment>> GetByRentalId(Guid rentalId)
    {
        return await _context.TbPayments
            .Where(p => p.RentalId == rentalId)
            .ToListAsync();
    }

    public async Task<TbPayment?> GetById(Guid id)
    {
        return await _context.TbPayments.FindAsync(id);
    }

    public async Task Add(TbPayment payment)
    {
        await _context.TbPayments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }
}
