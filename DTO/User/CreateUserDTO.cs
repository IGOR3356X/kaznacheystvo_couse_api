namespace KaznacheystvoCourse.DTO.User;

public class CreateUserDTO
{
    public string Login { get; set; }

    public string Password { get; set; } 

    public string FullName { get; set; }

    public string Email { get; set; }

    public int? RoleId { get; set; }
}