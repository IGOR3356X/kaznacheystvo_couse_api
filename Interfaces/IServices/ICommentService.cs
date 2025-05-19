using KaznacheystvoCourse.DTO.Comment;

namespace KaznacheystvoCourse.Interfaces.ISevices;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetCommentsForMaterialAsync(int learnMaterialId);
    Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto dto);
    Task DeleteCommentAsync(int commentId);
}