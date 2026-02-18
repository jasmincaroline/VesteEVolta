using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PostgresContext _context;

    public PaymentService(IPaymentRepository paymentRepository,
                          PostgresContext context)
    {
        _paymentRepository = paymentRepository;
        _context = context;
    }

    public async Task<List<TbPayment>> GetByRentalId(Guid rentalId)
    {
        return await _paymentRepository.GetByRentalId(rentalId);
    }

    public async Task<TbPayment> GetById(Guid id)
    {
        return await _paymentRepository.GetById(id);
    }

    public async Task<TbPayment> Create(Guid rentalId, CreatePaymentDto dto)
    {
        var rentalExists = await _context.TbRentals
            .AnyAsync(r => r.Id == rentalId);

        if (!rentalExists)
            throw new Exception("Aluguel não encontrado.");

        if (dto.Amount <= 0)
            throw new Exception("O valor deve ser maior que 0.");

        var allowedStatus = new[] { "pending", "paid", "canceled" };

        if (!allowedStatus.Contains(dto.PaymentStatus.ToLower()))
            throw new Exception("Status inválido.");

        var payment = new TbPayment
        {
            Id = Guid.NewGuid(),
            RentalId = rentalId,
            PaymentMethod = dto.PaymentMethod,
            Amount = dto.Amount,
            PaymentStatus = dto.PaymentStatus.ToLower(),
            PaymentDate = DateTime.UtcNow
        };

        await _paymentRepository.Add(payment);

        return payment;
    }
}
