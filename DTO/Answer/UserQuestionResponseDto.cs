using KaznacheystvoCourse.DTO.Option;

namespace KaznacheystvoCourse.DTO.Answer;

public class UserQuestionResponseDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string QuestionType { get; set; }
    public string UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public int Points { get; set; }
    public List<OptionDto> SelectedOptions { get; set; } = new();
}