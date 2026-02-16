using Microsoft.AspNetCore.Mvc;
using VesteEVolta.DTO;
using VesteEVolta.Services;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("rentals")]
public class RentalController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    // POST /api/rentals
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RentalDTO dto)
    {
        var rental = await _rentalService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = rental.Id },
            rental
        );
    }

    // GET /api/rentals
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rentals = await _rentalService.GetAll();
        return Ok(rentals);
    }

    // GET /api/rentals/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rental = await _rentalService.GetById(id);

        if (rental == null)
            return NotFound();

        return Ok(rental);
    }

    // PUT /api/rentals/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        await _rentalService.UpdateStatus(id, status);
        return NoContent();
    }

    // GET /api/users/{userId}/rentals
    [HttpGet("/api/users/{userId}/rentals")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var rentals = await _rentalService.GetByUserId(userId);
        return Ok(rentals);
    }

    // GET /api/clothes/{clothingId}/rentals
    [HttpGet("/api/clothes/{clothingId}/rentals")]
    public async Task<IActionResult> GetByClothing(Guid clothingId)
    {
        var rentals = await _rentalService.GetByClothingId(clothingId);
        return Ok(rentals);
    }
}
