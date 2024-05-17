using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string? Descriptions { get; set; }

    public string? Content { get; set; }

    public string? Slug { get; set; }

    public string TypeCourse { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public bool IsComing { get; set; }

    public bool IsPublish { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<CourseRevenue> CourseRevenues { get; set; } = new List<CourseRevenue>();

    public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
}
