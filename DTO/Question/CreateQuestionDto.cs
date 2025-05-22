using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.Option;

public class CreateQuestionDto
{
    [Required]
    public int MaterialId { get; set; }
    [Required]
    [StringLength(2055)]
    public string QuestionText { get; set; }

    [Required]
    public string QuestionType { get; set; }
    [Range(0, 100)]
    public int Points { get; set; } = 1; // Значение по умолчанию
    
    public string CorrectAnswer { get; set; }
    public List<OptionCreateDto> Options { get; set; } = new();
}