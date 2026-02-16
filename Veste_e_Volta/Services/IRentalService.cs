
using VesteEVolta.Models;
using VesteEVolta.DTO;

namespace VesteEVolta.Services;

public interface IRentalService
{
    Task<RentalResponseDTO> Create(RentalDTO dto);
    Task<RentalResponseDTO?> GetById(Guid id);
    Task<IEnumerable<RentalResponseDTO>> GetAll();
    Task<IEnumerable<RentalResponseDTO>> GetByUserId(Guid userId);
    Task<IEnumerable<RentalResponseDTO>> GetByClothingId(Guid clothingId);
    Task UpdateStatus(Guid id, string status);
}
