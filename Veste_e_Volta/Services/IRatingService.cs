using VesteEVolta.Application.DTOs;

public interface IRatingService
{
    Task CreateAsync(RatingDto dto);
    Task<List<RatingDto>> GetByClothingAsync(Guid clothingId);
    Task<List<RatingDto>> GetByUserAsync(Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<byte[]> GenerateReportAsync();

}
