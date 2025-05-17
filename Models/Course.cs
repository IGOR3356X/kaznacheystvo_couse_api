using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Ispublish { get; set; }

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
}
