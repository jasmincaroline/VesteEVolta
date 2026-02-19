using VesteEVolta.Models;
using VesteEVolta.DTO;
using VesteEVolta.Repositories;

namespace VesteEVolta.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IClothingRepository _clothingRepository;

    public RentalService(IRentalRepository rentalRepository, IClothingRepository clothingRepository)
    {
        _rentalRepository = rentalRepository;
        _clothingRepository = clothingRepository;
    }

    public async Task<RentalResponseDTO> Create(RentalDTO dto)
    {
        // Busca a roupa para obter o preço
        var clothing = await _clothingRepository.GetByIdAsync(dto.ClothingId);
        if (clothing == null)
            throw new Exception("Roupa não encontrada.");

        // Calcula o valor total (dias * preço por dia)
        var days = dto.EndDate.DayNumber - dto.StartDate.DayNumber;
        var totalValue = days * clothing.RentPrice;

        var rental = new TbRental
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ClothingId = dto.ClothingId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "active",
            TotalValue = totalValue,
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

    public async Task<RentalResponseDTO> Delete(Guid id)
    {
        var rental = await _rentalRepository.GetById(id);

        if (rental == null)
            throw new Exception("Aluguel não encontrado");

        await _rentalRepository.Delete(id);
        
        return MapToDTO(rental);
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
