namespace VesteEVolta.Application.DTOs;

public class RatingDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RentId { get; set; }

    public Guid ClothingId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateOnly Date { get; set; }

    public DateTime CreatedAt { get; set; }
}
