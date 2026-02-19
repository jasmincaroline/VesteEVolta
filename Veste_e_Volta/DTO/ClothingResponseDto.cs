namespace VesteEVolta.DTO
{
    public class ClothingResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal RentPrice { get; set; }
        public string AvailabilityStatus { get; set; } = null!;
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
