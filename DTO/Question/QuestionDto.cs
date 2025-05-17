using KaznacheystvoCourse.DTO.Option;

namespace KaznacheystvoCourse.DTO.Question;

public class QuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string QuestionType { get; set; }
    public string CorrectAnswer { get; set; }
    public int Points { get; set; }
    public int LearnMaterialId { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}