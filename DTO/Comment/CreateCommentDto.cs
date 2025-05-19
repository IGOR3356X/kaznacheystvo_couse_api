using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.Comment;

public class CreateCommentDto
{
    [Required]
    public string Text { get; set; }
    
    [Required]
    public int LearnMaterialId { get; set; }
    [Required]
    public int UserId { get; set; }
}