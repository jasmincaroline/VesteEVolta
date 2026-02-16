using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Application.DTOs;
using VesteEVolta.Services;
using VesteEVolta.Models;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("rating")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RatingDto dto)
    {
        await _ratingService.CreateAsync(dto);
        return Ok("Avaliação criada com sucesso.");
    }

    [HttpGet("clothing/{clothingId}")]
    public async Task<IActionResult> GetByClothing(Guid clothingId)
    {
        var ratings = await _ratingService.GetByClothingAsync(clothingId);
        return Ok(ratings);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid userId)
    {
        await _ratingService.DeleteAsync(id, userId);
        return NoContent();
    }
}
