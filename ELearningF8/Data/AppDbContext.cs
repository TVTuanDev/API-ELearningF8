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

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<BlackList> BlackLists { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3213E83F68D12704");

            entity.HasIndex(e => e.Slug, "UQ__Banners__32DD1E4C146FB8AE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(500)
                .HasColumnName("descriptions");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<BlackList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlackLis__3213E83FFAAD09C2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(500)
                .HasColumnName("accessToken");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chapters__3213E83FA583BD69");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.IdCourse).HasColumnName("idCourse");
            entity.Property(e => e.Sort).HasColumnName("sort");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdCourseNavigation).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.IdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chapters__update__26CFC035");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3213E83FDEDBDE53");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.IdPost).HasColumnName("idPost");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.ParentId).HasColumnName("parentId");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__idPost__4DE98D56");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Comments__parent__4CF5691D");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3213E83FD05245C6");

            entity.HasIndex(e => e.Slug, "UQ__Courses__32DD1E4CCA65797F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(500)
                .HasColumnName("descriptions");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lessons__3213E83F6273AC46");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(500)
                .HasColumnName("descriptions");
            entity.Property(e => e.IdChapter).HasColumnName("idChapter");
            entity.Property(e => e.Link).HasColumnName("link");
            entity.Property(e => e.LinkImg).HasColumnName("linkImg");
            entity.Property(e => e.LinkVideo).HasColumnName("linkVideo");
            entity.Property(e => e.Sort).HasColumnName("sort");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdChapterNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdChapter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__updateA__2AA05119");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3213E83FC03B1770");

            entity.HasIndex(e => e.Slug, "UQ__Posts__32DD1E4C40479897").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Descriptions)
                .HasMaxLength(500)
                .HasColumnName("descriptions");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Posts__idUser__1B5E0D89");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83F9A42D2FD");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.IdLesson).HasColumnName("idLesson");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.ParentId).HasColumnName("parentId");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdLessonNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.IdLesson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__idLes__52AE4273");

            entity.HasOne(d => d.Parent).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Questions__paren__51BA1E3A");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3213E83FEB8FF060");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expiredAt");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("isUsed");
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("issuedAt");
            entity.Property(e => e.JwtId)
                .HasMaxLength(50)
                .HasColumnName("jwtId");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__RefreshTo__idUse__3335971A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83FDFA4AB5F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("roleName");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83FFE834F5F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.BgAvatar).HasColumnName("bgAvatar");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.HasPassword)
                .HasMaxLength(100)
                .HasColumnName("hasPassword");
            entity.Property(e => e.IsLockedOut).HasColumnName("isLockedOut");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .HasColumnName("phone");
            entity.Property(e => e.Providers)
                .HasMaxLength(50)
                .HasColumnName("providers");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("twoFactorEnabled");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("userName");

            entity.HasMany(d => d.IdCourses).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("IdCourse")
                        .HasConstraintName("FK__UserCours__idCou__2E70E1FD"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK__UserCours__idUse__2D7CBDC4"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdCourse").HasName("PK__UserCour__BF8FE7B2B3232E8D");
                        j.ToTable("UserCourses");
                        j.IndexerProperty<int>("IdUser").HasColumnName("idUser");
                        j.IndexerProperty<int>("IdCourse").HasColumnName("idCourse");
                    });

            entity.HasMany(d => d.IdRoles).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRole")
                        .HasConstraintName("FK__UserRoles__idRol__1699586C"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .HasConstraintName("FK__UserRoles__idUse__15A53433"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdRole").HasName("PK__UserRole__69478C4712E6D7C6");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<int>("IdUser").HasColumnName("idUser");
                        j.IndexerProperty<int>("IdRole").HasColumnName("idRole");
                    });
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogi__3213E83F70409B56");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
            entity.Property(e => e.LoginProvider)
                .HasMaxLength(100)
                .HasColumnName("loginProvider");
            entity.Property(e => e.ProviderDisplayName)
                .HasMaxLength(200)
                .HasColumnName("providerDisplayName");
            entity.Property(e => e.ProviderKey)
                .HasMaxLength(200)
                .HasColumnName("providerKey");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__UserLogin__idUse__4183B671");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
