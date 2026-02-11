using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
using AulaApi.Veste_e_Volta.DTO;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase  
{
    private readonly PostgresContext _context;  

    public CategoriesController(PostgresContext context)  
    {
        _context = context;
    }

    [HttpGet] 
    public async Task<IActionResult> GetAll()   
    {
        var categories = await _context.TbCategories.ToListAsync();

        return Ok(categories); 
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryRequestDto dto)  
    {
        if (string.IsNullOrWhiteSpace(dto.Name))   
            return BadRequest("Nome é obrigatório.");

        var category = new TbCategory  
        {
            CategoryId = Guid.NewGuid(),
            Name = dto.Name
        };

        _context.TbCategories.Add(category);     
        await _context.SaveChangesAsync();        

        return Created("/api/categories", category);   
    }
}
