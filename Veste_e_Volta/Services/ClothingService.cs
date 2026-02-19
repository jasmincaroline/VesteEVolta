using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VesteEVolta.DTO;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;

namespace VesteEVolta.Services;

public class ClothingService  : IClothingService
{
    private readonly PostgresContext _context;

    public ClothingService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<ClothingResponseDto> CreateAsync(CreateClothingDto dto, ClaimsPrincipal user)
    {
        if (dto == null)
            throw new ArgumentException("Body inválido.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Descrição é obrigatória.");

        if (dto.RentPrice <= 0)
            throw new ArgumentException("RentPrice deve ser maior que zero.");

        // pega o userId do token (sub)
        var idClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idClaim) || !Guid.TryParse(idClaim, out var userId))
            throw new UnauthorizedAccessException("Token inválido.");


        // seu TbOwner tem PK = UserId
        var ownerExists = await _context.TbOwners.AnyAsync(o => o.UserId == userId);
        if (!ownerExists)
            throw new InvalidOperationException("Usuário não possui perfil Owner cadastrado.");

        var clothing = new TbClothing
        {
            Id = Guid.NewGuid(),
            Description = dto.Description.Trim(),
            RentPrice = dto.RentPrice,
            AvailabilityStatus = "AVAILABLE",
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TbClothings.AddAsync(clothing);
        await _context.SaveChangesAsync();

        return new ClothingResponseDto
        {
            Id = clothing.Id,
            Description = clothing.Description,
            RentPrice = clothing.RentPrice,
            AvailabilityStatus = clothing.AvailabilityStatus,
            OwnerId = clothing.OwnerId,
            CreatedAt = clothing.CreatedAt
        };
    }
}
