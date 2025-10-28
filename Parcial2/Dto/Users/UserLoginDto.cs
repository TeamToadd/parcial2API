using System.ComponentModel.DataAnnotations;

namespace Parcial2.Dto.Users;

public class UserLoginDto
{
    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}