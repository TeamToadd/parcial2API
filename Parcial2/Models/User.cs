using System.ComponentModel.DataAnnotations;

namespace Parcial2.Models;

public class User
{
    public int Id { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(256)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public Role Role { get; set; }

    [MaxLength(100)] public string? Name { get; set; }
    [MaxLength(100)] public string? LastName { get; set; }
    [MaxLength(100)] public string? UserName { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
    [MaxLength(50)]  public string? Phone { get; set; }
    [MaxLength(500)] public string? ProfileImageUrl { get; set; }

    // Solo Empresa
    [MaxLength(200)] public string? CompanyName { get; set; }

    public List<Order> Orders { get; set; } = [];
}