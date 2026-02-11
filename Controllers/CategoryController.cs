using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Services.Interfaces;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }
}
