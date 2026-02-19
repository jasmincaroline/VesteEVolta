using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VesteEVolta.DTO;
using VesteEVolta.Models;

namespace VesteEVolta.Controllers;

[ApiController]
[Route("users")]
//[Authorize]
public class UsersController : ControllerBase
{
    private readonly PostgresContext _context;

    public UsersController(PostgresContext context)
    {
        _context = context;
    }

    // GET /users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.TbUsers
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Telephone = u.Telephone,
                Email = u.Email,
                Reported = u.Reported,
                ProfileType = u.ProfileType,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET /users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var user = await _context.TbUsers
            .Where(u => u.Id == id)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Telephone = u.Telephone,
                Email = u.Email,
                Reported = u.Reported,
                ProfileType = u.ProfileType,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound("Usuário não encontrado.");

        return Ok(user);
    }

    // PUT /users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UserUpdateDto dto)
    {
        if (dto == null) return BadRequest("Body inválido.");
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email é obrigatório.");

        var user = await _context.TbUsers.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("Usuário não encontrado.");

        var email = dto.Email.Trim().ToLower();

        // 409 duplicado de email
        var duplicate = await _context.TbUsers.AnyAsync(u => u.Id != id && u.Email.ToLower() == email);
        if (duplicate) return Conflict("Email já cadastrado por outro usuário.");

        user.Name = dto.Name.Trim();
        user.Telephone = string.IsNullOrWhiteSpace(dto.Telephone) ? null : dto.Telephone.Trim();
        user.Email = email;

        await _context.SaveChangesAsync();

        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Telephone = user.Telephone,
            Email = user.Email,
            Reported = user.Reported,
            ProfileType = user.ProfileType,
            CreatedAt = user.CreatedAt
        });
    }

    // DELETE /users/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var user = await _context.TbUsers.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("Usuário não encontrado.");

        _context.TbUsers.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
