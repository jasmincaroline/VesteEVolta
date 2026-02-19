namespace VesteEVolta.DTO
{
    public class CreateReportDto
    {
        public Guid ReporterId { get; set; }
        public Guid ReportedId { get; set; }
        public Guid ReportedClothingId { get; set; }
        public Guid? RentalId { get; set; }

        public string Type { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string? Description { get; set; }
    }
}
