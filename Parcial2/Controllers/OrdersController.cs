using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial2.Data;
using Parcial2.Dto.Orders;
using Parcial2.Models;

namespace Parcial2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(AppDbContext db) : ControllerBase
{
    // Cliente: crear pedido con descuento de stock
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<OrderDto>> Create(OrderCreateDto dto)
    {
        var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var company = await db.Users.FirstOrDefaultAsync(u => u.Id == dto.CompanyUserId && u.Role == Role.Empresa);
        if (company == null) return BadRequest("Empresa no válida.");

        var neededIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products.Where(p => neededIds.Contains(p.Id)).ToListAsync();

        foreach (var it in dto.Items)
        {
            var prod = products.FirstOrDefault(p => p.Id == it.ProductId);
            if (prod == null || prod.CompanyUserId != dto.CompanyUserId)
                return BadRequest($"Producto {it.ProductId} no pertenece a la empresa.");
            if (prod.Stock < it.Quantity)
                return BadRequest($"Stock insuficiente para {prod.Name}. Disponible: {prod.Stock}");
        }

        var order = new Order
        {
            ClientUserId = clientId,
            CompanyUserId = dto.CompanyUserId,
            Status = OrderStatus.Nuevo,
            CreatedAt = DateTime.UtcNow
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        decimal total = 0m;
        foreach (var it in dto.Items)
        {
            var prod = products.First(p => p.Id == it.ProductId);
            prod.Stock -= it.Quantity;

            var item = new OrderItem
            {
                OrderId = order.Id,
                ProductId = prod.Id,
                Quantity = it.Quantity,
                UnitPrice = prod.Price
            };
            db.OrderItems.Add(item);

            total += prod.Price * it.Quantity;
        }
        order.Total = total;

        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, await ToDto(order.Id));
    }

    // Cliente: historial
    [HttpGet("mine")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> MyOrders()
    {
        var clientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await db.Orders.Where(o => o.ClientUserId == clientId).OrderByDescending(o => o.Id)
            .Select(o => new OrderDto
            {
                Id = o.Id, ClientUserId = o.ClientUserId, CompanyUserId = o.CompanyUserId,
                CreatedAt = o.CreatedAt, Status = o.Status, Total = o.Total,
                Items = o.Items.Select(i => new OrderDto.ItemDto
                { ProductId = i.ProductId, ProductName = i.Product!.Name, Quantity = i.Quantity, UnitPrice = i.UnitPrice }).ToList()
            }).ToListAsync();
        return Ok(list);
    }

    // Empresa: ver pedidos recibidos
    [HttpGet("company")]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> CompanyOrders()
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await db.Orders.Where(o => o.CompanyUserId == companyId).OrderByDescending(o => o.Id)
            .Select(o => new OrderDto
            {
                Id = o.Id, ClientUserId = o.ClientUserId, CompanyUserId = o.CompanyUserId,
                CreatedAt = o.CreatedAt, Status = o.Status, Total = o.Total,
                Items = o.Items.Select(i => new OrderDto.ItemDto
                { ProductId = i.ProductId, ProductName = i.Product!.Name, Quantity = i.Quantity, UnitPrice = i.UnitPrice }).ToList()
            }).ToListAsync();
        return Ok(list);
    }

    // Empresa: cambiar estado
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatusUpdateDto dto)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var o = await db.Orders.FirstOrDefaultAsync(x => x.Id == id && x.CompanyUserId == companyId);
        if (o == null) return NotFound();

        o.Status = dto.Status;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var dto = await ToDto(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }


    private async Task<OrderDto?> ToDto(int id)
    {
        return await db.Orders.Where(o => o.Id == id)
            .Select(o => new OrderDto
            {
                Id = o.Id, ClientUserId = o.ClientUserId, CompanyUserId = o.CompanyUserId,
                CreatedAt = o.CreatedAt, Status = o.Status, Total = o.Total,
                Items = o.Items.Select(i => new OrderDto.ItemDto
                { ProductId = i.ProductId, ProductName = i.Product!.Name, Quantity = i.Quantity, UnitPrice = i.UnitPrice }).ToList()
            }).FirstOrDefaultAsync();
    }
}
