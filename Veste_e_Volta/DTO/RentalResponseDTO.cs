namespace VesteEVolta.DTO;

public class RentalResponseDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClothingId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public decimal TotalValue { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
