using System.Security.Claims;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Services;

public interface IClothingService
{
    Task<ClothingResponseDto> CreateAsync(CreateClothingDto dto, ClaimsPrincipal user);
}
