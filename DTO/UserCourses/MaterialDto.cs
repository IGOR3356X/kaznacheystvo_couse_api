namespace KaznacheystvoCourse.DTO.UserCourses;

public class MaterialDto
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? AddedDate { get; set; }
    public int Progress { get; set; } // Процент прохождения
}