using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Response
{
    public int Id { get; set; }

    public string UserAnswer { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public int Points { get; set; }

    public int UserId { get; set; }

    public int ScoreId { get; set; }

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<ResponseOption> ResponseOptions { get; set; } = new List<ResponseOption>();

    public virtual Score Score { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
