using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.User;

public class UpdateUserDTO
{
    [MinLength(1)]
    public string? Login { get; set; }
    [MinLength(1)]
    public string? Password { get; set; } 
    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public IFormFile? Photo { get; set; }

    public int? RoleId { get; set; }
    
}