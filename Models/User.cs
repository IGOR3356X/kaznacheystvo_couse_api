using System;
using System.Collections.Generic;

namespace KaznacheystvoCourse.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Photo { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
}
