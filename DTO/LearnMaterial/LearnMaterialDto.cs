namespace KaznacheystvoCourse.DTO.LearnMaterial;

public class LearnMaterialDto
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public string File { get; set; }
    public int? ModuleId { get; set; }
    public string Video { get; set; }
    public string Code { get; set; }
}