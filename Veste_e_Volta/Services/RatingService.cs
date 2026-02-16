using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IRentalRepository _rentalRepository;

    public RatingService(
        IRatingRepository ratingRepository,
        IRentalRepository rentalRepository)
    {
        _ratingRepository = ratingRepository;
        _rentalRepository = rentalRepository;
    }

    public async Task CreateAsync(RatingDto dto)
    {
        var rent = await _rentalRepository.GetById(dto.RentId);

        if (rent == null)
            throw new Exception("Aluguel não encontrado.");

        if (rent.UserId != dto.UserId)
            throw new Exception("Você não pode avaliar este aluguel.");

        if (!rent.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase))
        throw new Exception("Só é possível avaliar após finalizar o aluguel.");


        var existing = await _ratingRepository.GetByRentIdAsync(dto.RentId);

        if (existing != null)
            throw new Exception("Este aluguel já foi avaliado.");

        if (rent.ClothingId != dto.ClothingId)
            throw new Exception("Clothing não corresponde ao aluguel.");

        var rating = new TbRating
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            RentId = dto.RentId,
            ClothingId = dto.ClothingId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await _ratingRepository.AddAsync(rating);
    }

    public async Task<List<RatingDto>> GetByClothingAsync(Guid clothingId)
    {
        var ratings = await _ratingRepository.GetByClothingIdAsync(clothingId);

        return ratings.Select(r => new RatingDto
        {
            Id = r.Id,
            UserId = r.UserId,
            RentId = r.RentId,
            ClothingId = r.ClothingId,
            Rating = r.Rating,
            Comment = r.Comment,
            Date = r.Date,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<List<RatingDto>> GetByUserAsync(Guid userId)
    {
        var ratings = await _ratingRepository.GetByUserIdAsync(userId);

        return ratings.Select(r => new RatingDto
        {
            Id = r.Id,
            UserId = r.UserId,
            RentId = r.RentId,
            ClothingId = r.ClothingId,
            Rating = r.Rating,
            Comment = r.Comment,
            Date = r.Date,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var rating = await _ratingRepository.GetByIdAsync(id);

        if (rating == null)
            throw new Exception("Avaliação não encontrada.");

        if (rating.UserId != userId)
            throw new Exception("Você não pode excluir essa avaliação.");

        await _ratingRepository.DeleteAsync(rating);
    }
}
