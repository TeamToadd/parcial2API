using System.ComponentModel.DataAnnotations;
using Parcial2.Models;

namespace Parcial2.Dto.Users;

public class UserRegisterDto
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(6)] public string Password { get; set; } = null!;
    [Required] public Role Role { get; set; }

    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }

    // requerido si Role=Empresa
    public string? CompanyName { get; set; }
}