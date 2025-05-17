using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Question
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? CorrectAnswer { get; set; }

    public string? QuestionType { get; set; }

    public int? LearnMaterialId { get; set; }

    public int Points { get; set; }

    public virtual ICollection<CorrectAnswer> CorrectAnswers { get; set; } = new List<CorrectAnswer>();

    public virtual LearnMaterial? LearnMaterial { get; set; }

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();
}
