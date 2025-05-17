using KaznacheystvoCourse.DTO.LearnMaterial;

namespace KaznacheystvoCourse.DTO.Module;

public class ModuleDto
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public int CourseId { get; set; }
    public List<LearnMaterialDtoForModule> LearnMaterials { get; set; } = new();
}