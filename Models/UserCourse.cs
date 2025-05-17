using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class UserCourse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
