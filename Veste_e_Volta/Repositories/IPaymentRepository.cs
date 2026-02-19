using VesteEVolta.Models;
public interface IPaymentRepository
{
    Task<List<TbPayment>> GetByRentalId(Guid rentalId);
    Task<TbPayment?> GetById(Guid id);
    Task Add(TbPayment payment);
}
