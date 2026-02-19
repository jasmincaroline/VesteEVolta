using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Application.DTOs;
using System.Text;

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
    [HttpGet("report")]
    public async Task<IActionResult> GenerateReport()
    {
    var fileContent = await _ratingService.GenerateReportAsync();

    var fileBytes = Encoding.UTF8.GetPreamble().Concat(fileContent).ToArray();

    return File(
        fileBytes,
        "text/csv",
        "rating-report.csv"
        );
    }
    
    [HttpGet("report/pdf")]
    public async Task<IActionResult> GeneratePdfReport()
    {
    var pdfContent = await _ratingService.GeneratePdfReportAsync();

    return File(
        pdfContent,
        "application/pdf",
        "rating-report.pdf"
    );
    }
}
