using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.Authorization;

public class AuthDTO
{
    [MinLength(1)] public string? Login { get; set; }
    [MinLength(1)] public string? Password { get; set; }
}