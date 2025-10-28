using System.ComponentModel.DataAnnotations;

namespace Parcial2.Models;

public class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int ClientUserId { get; set; }
    public User? ClientUser { get; set; }

    [Range(1,5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}