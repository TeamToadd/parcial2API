namespace Parcial2.Dto.Products;

public class ProductDto
{
    public int Id { get; set; }
    public int CompanyUserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public double AvgRating { get; set; }
}