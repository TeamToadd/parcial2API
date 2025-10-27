using System.ComponentModel.DataAnnotations;

namespace Parcial2.Dto.Products;

public class ProductCreateDto
{
    [Required] public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Range(0, 999999999)] public decimal Price { get; set; }
    [Range(0, int.MaxValue)] public int Stock { get; set; }
    public string? ImageUrl { get; set; }
}