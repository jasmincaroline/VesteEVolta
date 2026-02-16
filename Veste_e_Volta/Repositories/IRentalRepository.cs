using VesteEVolta.Models;

public interface IRentalRepository
{
    Task<TbRental> Add(TbRental rental);

    Task<TbRental?> GetById(Guid id);

    Task<IEnumerable<TbRental>> GetAll();

    Task<IEnumerable<TbRental>> GetByUserId(Guid userId);

    Task<IEnumerable<TbRental>> GetByClothingId(Guid clothingId);

    Task Update(TbRental rental);

    Task Delete(Guid id);
}
