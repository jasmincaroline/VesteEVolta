namespace VesteEVolta.DTO
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Telephone { get; set; }
        public string Email { get; set; } = null!;
        public bool Reported { get; set; }
        public string ProfileType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
