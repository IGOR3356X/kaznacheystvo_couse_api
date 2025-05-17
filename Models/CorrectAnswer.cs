using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class CorrectAnswer
{
    public int Id { get; set; }

    public int OptionId { get; set; }

    public int QuestionId { get; set; }

    public virtual Option Option { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
