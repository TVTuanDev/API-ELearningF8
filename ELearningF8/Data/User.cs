using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Bio { get; set; }

    public string? Phone { get; set; }

    public string? HasPassword { get; set; }

    public string Avatar { get; set; } = null!;

    public string BgAvatar { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool TwoFactorEnabled { get; set; }

    public string Providers { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<CourseRevenue> CourseRevenues { get; set; } = new List<CourseRevenue>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<Role> IdRoles { get; set; } = new List<Role>();
}
