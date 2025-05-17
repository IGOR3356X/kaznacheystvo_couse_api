using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.Course;

public class CreateUpdateCourseDto
{
    [Required]
    [StringLength(150)]
    public string Header { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }
    
    public bool IsPublished { get; set; }
}