using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.LearnMaterial;

public class CreateLearnMaterialDto
{
    [Required]
    [StringLength(250)]
    public string Header { get; set; }

    [Required]
    [StringLength(2055)]
    public string Description { get; set; }

    
    public IFormFile File { get; set; }

    public int? ModuleId { get; set; }
    
    [StringLength(2055)]
    public string Video { get; set; }
    
    public string Code { get; set; }
}