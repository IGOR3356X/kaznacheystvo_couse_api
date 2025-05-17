using System.ComponentModel.DataAnnotations;

namespace KaznacheystvoCourse.DTO.Option;

public class OptionCreateDto
{
    [Required]
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; } // Отмечаем правильные варианты
}