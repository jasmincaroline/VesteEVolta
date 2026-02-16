using VesteEVolta.Models;
public interface IPaymentService
{
    Task<List<TbPayment>> GetByRentalId(Guid rentalId);
    Task<TbPayment> GetById(Guid id);
    Task<TbPayment> Create(Guid rentalId, CreatePaymentDto dto);
}
