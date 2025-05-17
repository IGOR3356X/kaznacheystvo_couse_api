using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class TestQuestion
{
    public int TestId { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;
}
