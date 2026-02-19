using VesteEVolta.Models;

public interface IRatingRepository
{
    Task AddAsync(TbRating rating);
    Task<TbRating?> GetByIdAsync(Guid id);
    Task<TbRating?> GetByRentIdAsync(Guid rentId);
    Task<List<TbRating>> GetByClothingIdAsync(Guid clothingId);
    Task<List<TbRating>> GetByUserIdAsync(Guid userId);
    Task DeleteAsync(TbRating rating);
    Task<IEnumerable<TbRating>> GetAll();

}
