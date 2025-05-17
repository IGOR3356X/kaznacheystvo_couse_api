namespace KaznacheystvoCourse.DTO.Answer;

public class UserAnswerDto
{
    public int QuestionId { get; set; }
    public string TextAnswer { get; set; } // Для Text вопросов
    public List<int> SelectedOptionIds { get; set; } // Для CheckBox/RadioButton
}