using VesteEVolta.Application.DTOs;
using VesteEVolta.Models;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            throw new Exception("Roupa não corresponde ao aluguel.");

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
     public async Task<byte[]> GenerateReportAsync()
    {
        var ratings = await _ratingRepository.GetAll();

        var csvLines = new List<string> { "Id,UserId,ClothingId,Score,Comment" };
        foreach (var r in ratings)
        {
        var commentEscaped = r.Comment?.Replace("\"", "\"\"") ?? "";
        csvLines.Add($"{r.Id},{r.UserId},{r.ClothingId},{r.Rating},\"{commentEscaped}\"");
        }

        var csvContent = string.Join(Environment.NewLine, csvLines);
        return Encoding.UTF8.GetBytes(csvContent);
    }
    public async Task<byte[]> GeneratePdfReportAsync()
{
    var ratings = await _ratingRepository.GetAll();

    var pdfBytes = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.Content().Column(column =>
            {
                column.Item().Text("Rating Report").FontSize(20).Bold().AlignCenter();

                column.Item().LineHorizontal(1);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Id").Bold();
                    row.RelativeItem().Text("UserId").Bold();
                    row.RelativeItem().Text("ClothingId").Bold();
                    row.RelativeItem().Text("Rating").Bold();
                    row.RelativeItem().Text("Comment").Bold();
                });

                foreach (var r in ratings)
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text(r.Id.ToString());
                        row.RelativeItem().Text(r.UserId.ToString());
                        row.RelativeItem().Text(r.ClothingId.ToString());
                        row.RelativeItem().Text(r.Rating.ToString());
                        row.RelativeItem().Text(r.Comment ?? "");
                    });
                }
            });
        });
    }).GeneratePdf();

    return pdfBytes;
}
}
