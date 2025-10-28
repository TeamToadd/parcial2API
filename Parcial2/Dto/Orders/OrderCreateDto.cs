using System.ComponentModel.DataAnnotations;

namespace Parcial2.Dto.Orders;

public class OrderCreateDto
{
    [Required] public int CompanyUserId { get; set; }
    [Required] public List<OrderItemReq> Items { get; set; } = [];

    public class OrderItemReq
    {
        [Required] public int ProductId { get; set; }
        [Range(1,int.MaxValue)] public int Quantity { get; set; }
    }
}