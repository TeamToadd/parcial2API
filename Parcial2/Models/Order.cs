using System.ComponentModel.DataAnnotations.Schema;

namespace Parcial2.Models;

public class Order
{
    public int Id { get; set; }

    public int ClientUserId { get; set; }
    public User? ClientUser { get; set; }

    public int CompanyUserId { get; set; }
    public User? CompanyUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Nuevo;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}