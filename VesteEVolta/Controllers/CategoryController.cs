using Microsoft.AspNetCore.Mvc;
using VesteEVolta.Contracts.DTO;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto categoryDto)
    {
        categoryDto.CategoryId = id;
        await _service.UpdateAsync(categoryDto);
        var updatedCategory = await _service.GetByIdAsync(id);
        if (updatedCategory == null)
        {
            return NotFound();
        }
        return Ok(updatedCategory);
    }
}
