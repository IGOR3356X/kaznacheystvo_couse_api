using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class LearnMaterial
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? File { get; set; }

    public int? ModuleId { get; set; }

    public string? Video { get; set; }

    public string? Code { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Module? Module { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
