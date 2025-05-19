namespace KaznacheystvoCourse.DTO.User;

public class GetUserDTO
{
    public string Login { get; set; }

    public string Password { get; set; } 

    public string FullName { get; set; }
    
    public string? Photo { get; set; }

    public string Email { get; set; }
    public string Role { get; set; }
    
}