using VesteEVolta.Models;
using VesteEVolta.DTO;

namespace VesteEVolta.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<RentalResponseDTO> Create(RentalDTO dto)
    {
        var rental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ClothingId = dto.ClothingId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "active",
            TotalValue = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _rentalRepository.Add(rental);

        return MapToDTO(rental);
    }

    public async Task<RentalResponseDTO> GetById(Guid id)
    {
    var rental = await _rentalRepository.GetById(id);

    if (rental == null)
        throw new Exception("Aluguel não encontrado.");

    return MapToDTO(rental);
    }

    public async Task<IEnumerable<RentalResponseDTO>> GetAll()
    {
        var rentals = await _rentalRepository.GetAll();
        return rentals.Select(r => MapToDTO(r));
    }

    public async Task<IEnumerable<RentalResponseDTO>> GetByUserId(Guid userId)
    {
        var rentals = await _rentalRepository.GetByUserId(userId);
        return rentals.Select(r => MapToDTO(r));
    }

    public async Task<IEnumerable<RentalResponseDTO>> GetByClothingId(Guid clothingId)
    {
        var rentals = await _rentalRepository.GetByClothingId(clothingId);
        return rentals.Select(r => MapToDTO(r));
    }

    public async Task UpdateStatus(Guid id, string status)
    {
        var rental = await _rentalRepository.GetById(id);

        if (rental == null)
            throw new Exception("Aluguel não encontrado");

        rental.Status = status;

        await _rentalRepository.Update(rental);
    }

    private static RentalResponseDTO MapToDTO(TbRental rental)
    {
        return new RentalResponseDTO
        {
            Id = rental.Id,
            UserId = rental.UserId,
            ClothingId = rental.ClothingId,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalValue = rental.TotalValue,
            Status = rental.Status,
            CreatedAt = rental.CreatedAt,
        };
    }
}
