using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly PostgresContext _context;
    private readonly PasswordHasher<TbUser> _hasher;
    private readonly IConfiguration _configuration;

    public AuthController(PostgresContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _hasher = new PasswordHasher<TbUser>();
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] AuthRegisterDto dto)
    {
        var emailExists = _context.TbUsers.Any(u => u.Email == dto.Email);
        if (emailExists)
            return Conflict("Esse email já está cadastrado.");

        var user = new TbUser
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Telephone = dto.Telephone,
            Email = dto.Email,
            ProfileType = dto.ProfileType,
            Reported = false,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.TbUsers.Add(user);
        _context.SaveChanges();

        return Created("", new { user.Id, user.Name, user.Email, user.ProfileType });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthLoginDto dto)
    {
        var user = _context.TbUsers.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized("Email ou senha inválidos.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Email ou senha inválidos.");

        var token = GenerateToken(user);

        return Ok(new
        {
            token,
            userId = user.Id,
            profileType = user.ProfileType
        });
    }

    private string GenerateToken(TbUser user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.ProfileType)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
