using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ELearningF8.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<BlackList> BlackLists { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseRevenue> CourseRevenues { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionLesson> QuestionLessons { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TypeLesson> TypeLessons { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCourse> UserCourses { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3214EC0701144298");

            entity.ToTable("Answer");

            entity.Property(e => e.AnswerQuestion).HasMaxLength(200);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Explain).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdQuestionNavigation).WithMany(p => p.Answers)
                .HasForeignKey(d => d.IdQuestion)
                .HasConstraintName("FK__Answer__IdQuesti__5CD6CB2B");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC07438D82B9");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(500);
            entity.Property(e => e.Img).HasMaxLength(500);
            entity.Property(e => e.LinkButton).HasMaxLength(500);
            entity.Property(e => e.NameButton).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<BlackList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlackLis__3214EC07B610CBC0");

            entity.Property(e => e.AccessToken).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chapters__3214EC07FE777CFB");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.IdCourse)
                .HasConstraintName("FK__Chapters__IdCour__4BAC3F29");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC07519C6DBC");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdPost)
                .HasConstraintName("FK__Comments__IdPost__72C60C4A");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Comments__IdUser__71D1E811");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Comments__Parent__73BA3083");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC0740C947EA");

            entity.HasIndex(e => e.Slug, "UQ__Courses__BC7B5FB6E24113C9").IsUnique();

            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasDefaultValue("http://res.cloudinary.com/daeiiokje/image/upload/v1711273094/ELearningF8/Images/img_default_638468698928988378.png");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(500);
            entity.Property(e => e.Discount).HasColumnType("money");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseRevenue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC07EC551C29");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.CourseRevenues)
                .HasForeignKey(d => d.IdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseRev__IdCou__6754599E");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.CourseRevenues)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseRev__IdUse__68487DD7");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lessons__3214EC0793EF8D71");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Slug).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdChapterNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdChapter)
                .HasConstraintName("FK__Lessons__IdChapt__534D60F1");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__IdType__5441852A");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3214EC079350BA37");

            entity.HasIndex(e => e.Slug, "UQ__Posts__BC7B5FB6922264CA").IsUnique();

            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasDefaultValue("http://res.cloudinary.com/daeiiokje/image/upload/v1711273094/ELearningF8/Images/img_default_638468698928988378.png");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(500);
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__IdUser__6E01572D");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC073BFF1911");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdLessonNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdLesson)
                .HasConstraintName("FK__Questions__IdLes__787EE5A0");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Questions__IdUse__778AC167");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Questions__Paren__797309D9");
        });

        modelBuilder.Entity<QuestionLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC0714282FAC");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(200);
            entity.Property(e => e.Question).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdLessonNavigation).WithMany(p => p.QuestionLessons)
                .HasForeignKey(d => d.IdLesson)
                .HasConstraintName("FK__QuestionL__IdLes__5812160E");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0795CB5A2A");

            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.JwtId).HasMaxLength(50);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__RefreshTo__IdUse__03F0984C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC073A5FA3D8");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TypeLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeLess__3214EC07C483CFD8");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07DE48B8FC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105346509A286").IsUnique();

            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasDefaultValue("https://res.cloudinary.com/daeiiokje/image/upload/v1710573648/ELearningF8/Images/avartar%20default_638461704460223693.jpg");
            entity.Property(e => e.BgAvatar)
                .HasMaxLength(500)
                .HasDefaultValue("http://res.cloudinary.com/daeiiokje/image/upload/v1710606891/ELearningF8/Images/bg-Avatar_638462036889615906.png");
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HasPassword).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(12);
            entity.Property(e => e.Providers)
                .HasMaxLength(50)
                .HasDefaultValue("email");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("active");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasMany(d => d.IdRoles).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRole")
                        .HasConstraintName("FK__UserRoles__IdRol__4222D4EF"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK__UserRoles__IdUse__412EB0B6"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdRole").HasName("PK__UserRole__EC8A4F3DDBB29817");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<UserCourse>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdCourse }).HasName("PK__UserCour__B9C27680053E410E");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.IdCourse)
                .HasConstraintName("FK__UserCours__IdCou__619B8048");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__UserCours__IdUse__60A75C0F");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogi__3214EC07FCDEC2EF");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.LoginProvider)
                .HasMaxLength(100)
                .HasColumnName("loginProvider");
            entity.Property(e => e.ProviderDisplayName).HasMaxLength(100);
            entity.Property(e => e.ProviderKey).HasMaxLength(100);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__UserLogin__IdUse__0A9D95DB");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Videos__3214EC07941C544A");

            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
