using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<LearnMaterial> LearnMaterials { get; set; } = new List<LearnMaterial>();
}
