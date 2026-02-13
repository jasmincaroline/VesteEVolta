namespace VesteEVolta.DTO;

public class AuthRegisterDto
{
    public string Name { get; set; } = null!;
    public string? Telephone { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ProfileType { get; set; } = null!;
}