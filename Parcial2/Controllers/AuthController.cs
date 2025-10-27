using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Data;
using Parcial2.Dto.Users;
using Parcial2.Models;

namespace Parcial2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (!Enum.IsDefined(typeof(Role), dto.Role))
            return BadRequest("Rol inválido (Empresa=1, Cliente=2).");
        if (dto.Role == Role.Empresa && string.IsNullOrWhiteSpace(dto.CompanyName))
            return BadRequest("CompanyName requerido para Empresa.");

        var exists = await db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return Conflict("Email ya existe.");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = AppDbContext.Hash(dto.Password),
            Role = dto.Role,
            Name = dto.Name,
            LastName = dto.LastName,
            UserName = dto.UserName,
            Address = dto.Address,
            Phone = dto.Phone,
            ProfileImageUrl = dto.ProfileImageUrl,
            CompanyName = dto.CompanyName
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Created("", new { user.Id, user.Email, user.Role });
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login(UserLoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized("Credenciales inválidas.");
        if (!string.Equals(user.PasswordHash, AppDbContext.Hash(dto.Password), StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Credenciales inválidas.");

        var token = tokens.GenerateToken(user);
        return Ok(new { token });
    }
}