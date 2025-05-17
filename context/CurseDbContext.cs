using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCourse.Models;

public partial class CurseDbContext : DbContext
{
    public CurseDbContext()
    {
    }

    public CurseDbContext(DbContextOptions<CurseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CorrectAnswer> CorrectAnswers { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<LearnMaterial> LearnMaterials { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Response> Responses { get; set; }

    public virtual DbSet<ResponseOption> ResponseOptions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCourse> UserCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=94.156.112.206;Port=5432;Database=curse_db;Username=netherkick;Password=mrpopa#666@");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("C");

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comment_pkey");

            entity.ToTable("comment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date_time");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("comment_user_id_fkey");
        });

        modelBuilder.Entity<CorrectAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("correct_answers_pkey");

            entity.ToTable("correct_answers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Option).WithMany(p => p.CorrectAnswers)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("correct_answers_option_id_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.CorrectAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("correct_answers_question_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_pkey");

            entity.ToTable("course");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Header)
                .HasMaxLength(150)
                .HasColumnName("header");
            entity.Property(e => e.Ispublish).HasColumnName("ispublish");
        });

        modelBuilder.Entity<LearnMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("learn_material_pkey");

            entity.ToTable("learn_material");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(2055)
                .HasColumnName("description");
            entity.Property(e => e.File).HasColumnName("file");
            entity.Property(e => e.Header)
                .HasMaxLength(250)
                .HasColumnName("header");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Video)
                .HasMaxLength(2055)
                .HasColumnName("video");

            entity.HasOne(d => d.Module).WithMany(p => p.LearnMaterials)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("learn_material_module_id_fkey");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("module_pkey");

            entity.ToTable("module");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Header)
                .HasMaxLength(150)
                .HasColumnName("header");

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("module_course_id_fkey");
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("option_pkey");

            entity.ToTable("option");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("option_question_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("question_pkey");

            entity.ToTable("question");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(250)
                .HasColumnName("correct_answer");
            entity.Property(e => e.LearnMaterialId).HasColumnName("learn_material_id");
            entity.Property(e => e.Points)
                .HasDefaultValue(0)
                .HasColumnName("points");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(2055)
                .HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(20)
                .HasColumnName("question_type");

            entity.HasOne(d => d.LearnMaterial).WithMany(p => p.Questions)
                .HasForeignKey(d => d.LearnMaterialId)
                .HasConstraintName("question_learn_material_id_fkey");
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("response_pkey");

            entity.ToTable("response");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.UserAnswer).HasColumnName("user_answer");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Responses)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("response_question_id_fkey");

            entity.HasOne(d => d.Score).WithMany(p => p.Responses)
                .HasForeignKey(d => d.ScoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("score_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Responses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("response_user_id_fkey");
        });

        modelBuilder.Entity<ResponseOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("response_option_pkey");

            entity.ToTable("response_option");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.ResponseId).HasColumnName("response_id");

            entity.HasOne(d => d.Option).WithMany(p => p.ResponseOptions)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("response_option_option_id_fkey");

            entity.HasOne(d => d.Response).WithMany(p => p.ResponseOptions)
                .HasForeignKey(d => d.ResponseId)
                .HasConstraintName("response_option_response_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("score_pkey");

            entity.ToTable("score");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LearnMaterialId).HasColumnName("learn_material_id");
            entity.Property(e => e.Score1).HasColumnName("score");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.LearnMaterial).WithMany(p => p.Scores)
                .HasForeignKey(d => d.LearnMaterialId)
                .HasConstraintName("score_learn_material_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Scores)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("score_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_courses_pkey");

            entity.ToTable("user_courses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("user_courses_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_courses_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
