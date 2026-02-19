using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.Models;
using VesteEVolta.DTO;
using VesteEVolta.Services;

namespace VesteEVolta.Controllers;


[ApiController]
[Route("categories")]
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var category = await _context.TbCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound("Categoria não encontrada :(");

        return Ok(category);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da categoria são obrigatórios.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Nome é obrigatório, não esquece :)");

        // 409 - nome duplicado
        var exists = await _context.TbCategories
            .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
        if (exists)
            return Conflict("Já existe uma categoria com esse nome, escolha outro nome, por favor :)");

        var category = new TbCategory
        {
            CategoryId = Guid.NewGuid(),
            Name = dto.Name
        };

        _context.TbCategories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
    }

    [Authorize]
    [HttpPut("{id:guid}")] //Atualiza uma categoria existente
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CategoryRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da categoria são obrigatórios.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Nome é obrigatório, não esquece :)");

        var category = await _context.TbCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound("Categoria não encontrada :(");

        var newName = dto.Name.Trim();

        // 409 - nome duplicado
        var duplicate = await _context.TbCategories
            .AnyAsync(c => c.CategoryId != id && c.Name.ToLower() == newName.ToLower());

        if (duplicate)
            return Conflict("Já existe uma categoria com esse nome, escolha outro nome, por favor :)");

        category.Name = newName;

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var category = await _context.TbCategories
            .Include(c => c.Clothings) // Include traz as roupas junto
            .FirstOrDefaultAsync(c => c.CategoryId == id);
           
        if (category == null)
            return NotFound("Categoria não encontrada :(");

        //se a categoria estiver associada a roupas, não permitir a exclusão
        if (category.Clothings.Any())
            return Conflict("Não é possível deletar, pois a categoria está associada a uma roupa.");
        
        _context.TbCategories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
            
    

