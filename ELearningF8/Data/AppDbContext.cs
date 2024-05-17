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

    public virtual DbSet<PostTag> PostTags { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionLesson> QuestionLessons { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TypeLesson> TypeLessons { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCourse> UserCourses { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3214EC07380944AA");

            entity.ToTable("Answer");

            entity.Property(e => e.AnswerQuestion).HasMaxLength(200);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Explain).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdQuestionNavigation).WithMany(p => p.Answers)
                .HasForeignKey(d => d.IdQuestion)
                .HasConstraintName("FK__Answer__IdQuesti__60A75C0F");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC07A78141F4");

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
            entity.HasKey(e => e.Id).HasName("PK__BlackLis__3214EC0719787D8F");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chapters__3214EC0735632D53");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.IdCourse)
                .HasConstraintName("FK__Chapters__IdCour__4F7CD00D");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC07BCFD77DC");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdPost)
                .HasConstraintName("FK__Comments__IdPost__01142BA1");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Comments__IdUser__00200768");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Comments__Parent__02084FDA");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3214EC07B1E09C9A");

            entity.HasIndex(e => e.Slug, "UQ__Courses__BC7B5FB6FE11DA56").IsUnique();

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
            entity.Property(e => e.TypeCourse)
                .HasMaxLength(50)
                .HasDefaultValue("free");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseRevenue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseRe__3214EC0784EE2C70");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.CourseRevenues)
                .HasForeignKey(d => d.IdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseRev__IdCou__6B24EA82");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.CourseRevenues)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseRev__IdUse__6C190EBB");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lessons__3214EC07A250037F");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Slug).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdChapterNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdChapter)
                .HasConstraintName("FK__Lessons__IdChapt__571DF1D5");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__IdType__5812160E");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3214EC073ABC76FC");

            entity.HasIndex(e => e.Slug, "UQ__Posts__BC7B5FB62A439B9C").IsUnique();

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
                .HasConstraintName("FK__Posts__IdUser__72C60C4A");
        });

        modelBuilder.Entity<PostTag>(entity =>
        {
            entity.HasKey(e => new { e.IdPost, e.IdTag }).HasName("PK__PostTags__5A60DDFD145BF7A8");

            entity.Property(e => e.IdTag).HasDefaultValue(1);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.IdPost)
                .HasConstraintName("FK__PostTags__IdPost__7B5B524B");

            entity.HasOne(d => d.IdTagNavigation).WithMany(p => p.PostTags)
                .HasForeignKey(d => d.IdTag)
                .HasConstraintName("FK__PostTags__IdTag__7C4F7684");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC079F42172E");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdLessonNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdLesson)
                .HasConstraintName("FK__Questions__IdLes__06CD04F7");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Questions__IdUse__05D8E0BE");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Questions__Paren__07C12930");
        });

        modelBuilder.Entity<QuestionLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC070B2688D4");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(200);
            entity.Property(e => e.Question).HasMaxLength(200);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.IdLessonNavigation).WithMany(p => p.QuestionLessons)
                .HasForeignKey(d => d.IdLesson)
                .HasConstraintName("FK__QuestionL__IdLes__5BE2A6F2");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0799E7C967");

            entity.Property(e => e.AccessId).HasMaxLength(50);
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__RefreshTo__IdUse__123EB7A3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07E5F3F009");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC075D78EC4A");

            entity.HasIndex(e => e.TagName, "UQ__Tags__BDE0FD1D580CE395").IsUnique();

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.TagName).HasMaxLength(100);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<TypeLesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeLess__3214EC07ECF8B536");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");
            entity.Property(e => e.TypeName).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC072F811256");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534B3DDCF45").IsUnique();

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
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasDefaultValue("Guest");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserCourse>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdCourse }).HasName("PK__UserCour__B9C27680C7F70688");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.IdCourse)
                .HasConstraintName("FK__UserCours__IdCou__656C112C");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserCourses)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__UserCours__IdUse__6477ECF3");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogi__3214EC07CDEC6FAC");

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
                .HasConstraintName("FK__UserLogin__IdUse__18EBB532");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdRole }).HasName("PK__UserRole__EC8A4F3D1FB189E5");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdRole)
                .HasConstraintName("FK__UserRoles__IdRol__440B1D61");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__UserRoles__IdUse__4316F928");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Videos__3214EC07B791D085");

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
