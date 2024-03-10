using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ELearningF8.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:AppDbContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banner__3213E83F69FC5D53");

            entity.ToTable("Banner");

            entity.HasIndex(e => e.Slug, "UQ__Banner__32DD1E4CADBA290A").IsUnique();

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

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chapters__3213E83F3E6A10B5");

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
                .HasConstraintName("FK__Chapters__update__10216507");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3213E83FEEC6A6B9");

            entity.HasIndex(e => e.Slug, "UQ__Courses__32DD1E4CC2B9834D").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Lessons__3213E83FCACDB8FF");

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
                .HasConstraintName("FK__Lessons__updateA__13F1F5EB");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Posts__3213E83F8EADCC70");

            entity.HasIndex(e => e.Slug, "UQ__Posts__32DD1E4CFACA8DBA").IsUnique();

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__idUser__251C81ED");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3213E83FE6CEDA11");

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__idUse__3B0BC30C");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F873CA5CC");

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
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F7C169A89");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616452D55C89").IsUnique();

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
                .IsUnicode(false)
                .HasColumnName("phone");
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
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserCours__idCou__28ED12D1"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserCours__idUse__27F8EE98"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdCourse").HasName("PK__UserCour__BF8FE7B2E972B3D2");
                        j.ToTable("UserCourse");
                        j.IndexerProperty<int>("IdUser").HasColumnName("idUser");
                        j.IndexerProperty<int>("IdCourse").HasColumnName("idCourse");
                    });

            entity.HasMany(d => d.IdRoles).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRole")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__idRole__2057CCD0"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__idUser__1F63A897"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdRole").HasName("PK__UserRole__69478C47AB61CF12");
                        j.ToTable("UserRole");
                        j.IndexerProperty<int>("IdUser").HasColumnName("idUser");
                        j.IndexerProperty<int>("IdRole").HasColumnName("idRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
