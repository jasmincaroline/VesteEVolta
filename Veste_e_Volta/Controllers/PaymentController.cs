using Microsoft.AspNetCore.Mvc;
using VesteEVolta.DTO;
using VesteEVolta.Services;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByRental([FromQuery] Guid rentalId)
    {
        var payments = await _paymentService.GetByRentalId(rentalId);
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetById(id);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var payment = await _paymentService.Create(dto.RentalId, dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = payment.Id },
                payment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
