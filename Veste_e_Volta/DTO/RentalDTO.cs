namespace VesteEVolta.DTO;

public class RentalDTO
{
    public Guid UserId { get; set; }
    public Guid ClothingId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
