using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.UserCourses;

public class AddMaterialRequest
{
    [Required]
    public int CourseId { get; set; }
}