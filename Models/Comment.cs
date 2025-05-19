using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public int LearnMaterialId { get; set; }

    public virtual LearnMaterial LearnMaterial { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
