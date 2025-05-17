using KaznacheystvoCourse.DTO.Option;

namespace KaznacheystvoCourse.DTO.Answer;

public class UserAttemptDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int TotalScore { get; set; }
    public DateTime AttemptDate { get; set; }
    public List<UserQuestionResponseDto> Responses { get; set; } = new();
}