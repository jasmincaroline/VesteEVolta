using Microsoft.AspNetCore.Mvc;
using VesteEVolta.DTO;
using VesteEVolta.Services.Interfaces;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("API funcionando");
    }
}
