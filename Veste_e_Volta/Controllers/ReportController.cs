using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VesteEVolta.DTO;
using VesteEVolta.Services;
using VesteEVolta.Models;

namespace VesteEVolta.Controllers
{
    [ApiController]
    [Route("reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;
        private readonly PostgresContext _context;

        public ReportsController(IReportService service, PostgresContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportDto dto)
        {
            var userIdStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var reporterId))
                return Unauthorized("Token sem userId válido.");

            var created = await _service.CreateAsync(reporterId, dto);

            //aqui troca Id -> ReportId
            return CreatedAtAction(nameof(GetById), new { id = created.ReportId }, created);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var report = await _service.GetByIdAsync(id);
            return report is null ? NotFound() : Ok(report);
        }


        [HttpGet("rentals")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetRentalsCsv([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            if (from > to)
                return BadRequest("'from' não pode ser maior que 'to'.");

            var rentals = await _context.TbRentals
                .Include(r => r.Clothing)
                .Include(r => r.User)
                .Where(r => r.StartDate >= from && r.StartDate <= to)
                .OrderBy(r => r.StartDate)
                .ToListAsync(); 

            var sb = new System.Text.StringBuilder();

            // Header do CSV
            sb.AppendLine("RentalId,UserId,UserName,ClothingId,Description,StartDate,EndDate,Status,TotalValue");

            foreach (var r in rentals)
            {
                sb.AppendLine(
                    $"{r.Id}," +
                    $"{r.UserId}," +
                    $"{r.User.Name}," +
                    $"{r.ClothingId}," +
                    $"{r.Clothing.Description}," +
                    $"{r.StartDate:yyyy-MM-dd}," +
                    $"{r.EndDate:yyyy-MM-dd}," +
                    $"{r.Status}," +
                    $"{r.TotalValue}"
                );
            }

            var bytes = new System.Text.UTF8Encoding(true).GetBytes(sb.ToString());

            return File(bytes, "text/csv", $"rentals_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
        }


        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateReportStatusDto dto)
        {
            var updated = await _service.UpdateStatusAsync(id, dto.Status);
            return Ok(updated);
        }



    }
}
