using KaznacheystvoCourse.DTO.User;

namespace KaznacheystvoCourse.DTO.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public UserInfoDto Author { get; set; }
}