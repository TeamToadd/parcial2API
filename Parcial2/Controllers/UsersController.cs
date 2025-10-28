using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Data;
using Parcial2.Dto.Users;

namespace Parcial2.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserInfoDto>> Me()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var u = await db.Users.FindAsync(id);
        if (u == null) return NotFound();
        return new UserInfoDto { Id = u.Id, Email = u.Email, Role = u.Role, Name = u.Name, CompanyName = u.CompanyName };
    }
}