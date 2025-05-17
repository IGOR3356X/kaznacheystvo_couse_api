using KaznacheystvoCourse.DTO.Answer;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface ITestService
{
    Task<TestResultDto> SubmitTestAnswersAsync(int userId, SubmitTestDto dto);
    Task<TestResultDto> GetTestResultAsync(int scoreId);
    Task<IEnumerable<UserAttemptDto>> GetAllResponsesForMaterialAsync(int materialId);
}