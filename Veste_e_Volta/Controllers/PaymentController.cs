using Microsoft.AspNetCore.Mvc;
using VesteEVolta.DTO;
using VesteEVolta.Services;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("rentals/{rentalId}/payments")]
    public async Task<IActionResult> GetByRental(Guid rentalId)
    {
        var payments = await _paymentService.GetByRentalId(rentalId);
        return Ok(payments);
    }

    [HttpGet("payments/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _paymentService.GetById(id);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpPost("rentals/{rentalId}/payments")]
    public async Task<IActionResult> Create(Guid rentalId,
    [FromBody] CreatePaymentDto dto)
    {
        try
        {
            var payment = await _paymentService.Create(rentalId, dto);

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
