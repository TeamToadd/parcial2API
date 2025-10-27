using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Data;
using Parcial2.Dto.Products;
using Parcial2.Models;

namespace Parcial2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext db) : ControllerBase
{
    // Cliente: listar y filtrar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery]int? companyId, [FromQuery]decimal? minPrice, [FromQuery]decimal? maxPrice)
    {
        var q = db.Products.AsQueryable();
        if (companyId.HasValue) q = q.Where(p => p.CompanyUserId == companyId);
        if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice);
        if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice);

        var list = await q
            .Select(p => new ProductDto
            {
                Id = p.Id,
                CompanyUserId = p.CompanyUserId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                AvgRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
            }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("companies")]
    public async Task<ActionResult<IEnumerable<object>>> Companies()
    {
        var companies = await db.Users
            .Where(u => u.Role == Role.Empresa)
            .Select(u => new { u.Id, u.CompanyName, u.Email })
            .ToListAsync();
        return Ok(companies);
    }

    // Empresa: CRUD propio
    [HttpPost]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var p = new Product
        {
            CompanyUserId = companyId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl
        };
        db.Products.Add(p);
        await db.SaveChangesAsync();

        return Created("", new ProductDto
        {
            Id = p.Id, CompanyUserId = companyId, Name = p.Name, Description = p.Description,
            Price = p.Price, Stock = p.Stock, ImageUrl = p.ImageUrl, AvgRating = 0
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> Update(int id, ProductCreateDto dto)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var p = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.CompanyUserId == companyId);
        if (p == null) return NotFound();

        p.Name = dto.Name;
        p.Description = dto.Description;
        p.Price = dto.Price;
        p.Stock = dto.Stock;
        p.ImageUrl = dto.ImageUrl;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var p = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.CompanyUserId == companyId);
        if (p == null) return NotFound();

        db.Products.Remove(p);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
