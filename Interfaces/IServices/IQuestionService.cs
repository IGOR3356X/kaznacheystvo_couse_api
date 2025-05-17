using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.DTO.Question;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface IQuestionService
{
    Task<QuestionDto> GetQuestionAsync(int id);
    Task<IEnumerable<QuestionDto>> GetQuestionsByMaterialAsync(int materialId);
    Task<QuestionDto> CreateQuestionAsync(int materialId, CreateQuestionDto dto);
    Task<QuestionDto> UpdateQuestionAsync(int id,UpdateQuestionDto dto);
    Task DeleteQuestionAsync(int id);
}