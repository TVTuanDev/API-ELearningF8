using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string HasPassword { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? BgAvatar { get; set; }

    public bool IsLockedOut { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Course> IdCourses { get; set; } = new List<Course>();

    public virtual ICollection<Role> IdRoles { get; set; } = new List<Role>();
}
