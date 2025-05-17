using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.User;

public class UpdateUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; } 
    public string? FullName { get; set; }

    [Phone]
    public string? Telephone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public IFormFile? Photo { get; set; }

    public int? RoleId { get; set; }
    
}