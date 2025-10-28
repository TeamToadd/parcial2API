using Parcial2.Models;

namespace Parcial2.Dto.Users;

public class UserInfoDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
    public string? Name { get; set; }
    public string? CompanyName { get; set; }
}