using Parcial2.Models;

namespace Parcial2.Dto.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public int ClientUserId { get; set; }
    public int CompanyUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public List<ItemDto> Items { get; set; } = [];

    public class ItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}