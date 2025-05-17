using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Module
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<LearnMaterial> LearnMaterials { get; set; } = new List<LearnMaterial>();
}
