using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("test")]
public class TestAuthController : ControllerBase
{
    [Authorize]
    [HttpGet("any")]
    public IActionResult AnyLogged()
        => Ok("Qualquer pessoa logada ✅");

    [Authorize(Roles = "Owner")]
    [HttpGet("owner")]
    public IActionResult OnlyOwner()
        => Ok("Somente Owner ✅");

    [Authorize(Roles = "User")]
    [HttpGet("user")]
    public IActionResult OnlyUser()
        => Ok("Somente User ✅");
}
