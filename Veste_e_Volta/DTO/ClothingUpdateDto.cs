namespace VesteEVolta.DTO
{
    public class ClothingUpdateDto
    {
        public string Description { get; set; } = null!;
        public decimal RentPrice { get; set; }
        public string AvailabilityStatus { get; set; } = "AVAILABLE";
    }
}
