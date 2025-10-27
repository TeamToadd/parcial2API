using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Data;
using Parcial2.Dto.Reviews;

namespace Parcial2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Create(ReviewCreateDto dto)
    {
        var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var purchased = await db.Orders
            .Where(o => o.ClientUserId == clientId && o.Items.Any(i => i.ProductId == dto.ProductId))
            .AnyAsync();

        if (!purchased) return BadRequest("Solo puedes reseñar productos que compraste.");

        var already = await db.Reviews.AnyAsync(r => r.ProductId == dto.ProductId && r.ClientUserId == clientId);
        if (already) return Conflict("Ya reseñaste este producto.");

        db.Reviews.Add(new Parcial2.Models.Review
        {
            ProductId = dto.ProductId,
            ClientUserId = clientId,
            Rating = dto.Rating,
            Comment = dto.Comment
        });
        await db.SaveChangesAsync();
        return Created("", new { ok = true });
    }

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> ListByProduct(int productId)
    {
        var list = await db.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Rating, r.Comment, r.CreatedAt, r.ClientUserId })
            .ToListAsync();
        return Ok(list);
    }
}