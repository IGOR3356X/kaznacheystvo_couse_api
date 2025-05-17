using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.LearnMaterial;

public class CreateUpdateModuleDto
{
    [Required]
    [StringLength(150)]
    public string Header { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; }

    [Required]
    public int CourseId { get; set; }
}