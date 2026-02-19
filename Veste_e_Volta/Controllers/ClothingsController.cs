using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.DTO;
using VesteEVolta.Models;
using VesteEVolta.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace VesteEVolta.Controllers;

[ApiController]
[Route("clothes")]
public class ClothingsController : ControllerBase
{
    private readonly PostgresContext _context; 
    private readonly IClothingService _clothingService;

    public ClothingsController(PostgresContext context, IClothingService clothingService)
    {
        _context = context;
        _clothingService = clothingService;
    }

    //JOIN: GET /clothes (traz roupas + categorias)
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] string? status,
    [FromQuery] Guid? categoryId,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice)
    {
        var query = _context.TbClothings
            .Include(c => c.Categories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpper();
            query = query.Where(c => c.AvailabilityStatus.ToUpper() == normalizedStatus);
        }

        if (minPrice.HasValue)
            query = query.Where(c => c.RentPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.RentPrice <= maxPrice.Value);

        if (categoryId.HasValue)
            query = query.Where(c => c.Categories.Any(cat => cat.CategoryId == categoryId.Value));

        var clothes = await query
            .Select(c => new
            {
                c.Id,
                c.Description,
                c.RentPrice,
                c.AvailabilityStatus,
                c.OwnerId,
                Categories = c.Categories.Select(cat => new
                {
                    cat.CategoryId,
                    cat.Name
                })
            })
            .ToListAsync();

        return Ok(clothes);
    }

    // GET /clothes/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var clothing = await _context.TbClothings
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clothing == null)
            return NotFound("Roupa não encontrada.");

        return Ok(new
        {
            clothing.Id,
            clothing.Description,
            clothing.RentPrice,
            clothing.AvailabilityStatus,
            clothing.OwnerId,
            Categories = clothing.Categories.Select(cat => new { cat.CategoryId, cat.Name })
        });
    }


    // GET /clothes/{id}/categories
    [HttpGet("{id:guid}/categories")]
    public async Task<IActionResult> GetCategories([FromRoute] Guid id)
    {
        var clothing = await _context.TbClothings
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clothing == null)
            return NotFound("Roupa não encontrada.");

        // retorna só as categorias 
        var result = clothing.Categories.Select(cat => new
        {
            cat.CategoryId,
            cat.Name
        });

        return Ok(result);
    }

    
    // PUT /clothes/{id}/categories -> substitui todas as categorias dessa roupa
    [Authorize(Roles = "Owner")] 
    [HttpPut("{id:guid}/categories")]
    public async Task<IActionResult> UpdateCategories([FromRoute] Guid id, [FromBody] ClothingCategoriesRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Body inválido.");

        if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
            return BadRequest("Informe ao menos uma categoria.");

        // carrega a roupa + categorias atuais
        var clothing = await _context.TbClothings
            .Include(c => c.Categories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (clothing == null)
            return NotFound("Roupa não encontrada.");

        // busca categorias que existem no BD
        var categories = await _context.TbCategories
            .Where(cat => dto.CategoryIds.Contains(cat.CategoryId))
            .ToListAsync();

        // se mandou algum id que não existe, devolve 404
        if (categories.Count != dto.CategoryIds.Distinct().Count())
            return NotFound("Uma ou mais categorias não foram encontradas.");

        // substitui as categorias
        clothing.Categories.Clear();
        foreach (var cat in categories)
            clothing.Categories.Add(cat);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            clothing.Id,
            Categories = clothing.Categories.Select(c => new { c.CategoryId, c.Name })
        });
    }
    
    // PUT /clothes/{id} -> atualiza os dados da roupa
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ClothingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto == null)
            return BadRequest("Body inválido.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            return BadRequest("Descrição é obrigatória.");

        if (dto.RentPrice <= 0)
            return BadRequest("Valor do aluguel deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(dto.AvailabilityStatus))
            return BadRequest("AvailabilityStatus é obrigatório.");

        // pega o userId do token 
        if (!TryGetUserId(out var userId))
            return Unauthorized("Token inválido.");


        var clothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == id);
        if (clothing == null)
            return NotFound("Roupa não encontrada.");

        // só o dono pode editar
        if (clothing.OwnerId != userId)
            return Forbid(); // 403

        clothing.Description = dto.Description.Trim();
        clothing.RentPrice = dto.RentPrice;
        clothing.AvailabilityStatus = dto.AvailabilityStatus.Trim().ToUpper();

        await _context.SaveChangesAsync();

        return Ok(new
        {
            clothing.Id,
            clothing.Description,
            clothing.RentPrice,
            clothing.AvailabilityStatus,
            clothing.OwnerId
        });
    }


    //POST /clothes -> cria roupa nova
    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClothingDto dto)
    {
        var result = await _clothingService.CreateAsync(dto, User);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // DELETE /clothes/{id} 
    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Token inválido.");

        var clothing = await _context.TbClothings.FirstOrDefaultAsync(c => c.Id == id);
        if (clothing == null)
            return NotFound("Roupa não encontrada :(.");

        // só o dono pode deletar
        if (clothing.OwnerId != userId)
            return Forbid();

        // 409 se estiver alugada
        if (string.Equals(clothing.AvailabilityStatus, "RENTED", StringComparison.OrdinalIgnoreCase))
            return Conflict("Não é possível deletar uma roupa alugada (RENTED).");

        _context.TbClothings.Remove(clothing);
        await _context.SaveChangesAsync();

        return NoContent(); // 204
    }
    private bool TryGetUserId(out Guid userId)
    {
        var idStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(idStr, out userId);
    }


}
