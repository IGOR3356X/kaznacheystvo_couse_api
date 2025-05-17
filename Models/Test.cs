using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Test
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaxScore { get; set; }

    public int? LearnMaterialId { get; set; }

    public virtual LearnMaterial? LearnMaterial { get; set; }

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
