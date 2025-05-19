namespace KaznacheystvoCourse.DTO.Answer;

public class TestResultDto
{
    public int ScoreId { get; set; }
    public int TotalScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<UserQuestionResponseDto> Responses { get; set; } // Добавлена детализация
}