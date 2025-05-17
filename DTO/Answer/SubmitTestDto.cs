namespace KaznacheystvoCourse.DTO.Answer;

public class SubmitTestDto
{
    public int UserId { get; set; }
    public int LearnMaterialId { get; set; }
    public List<UserAnswerDto> Answers { get; set; }
}