using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Descriptions { get; set; }

    public string? Content { get; set; }

    public string? Slug { get; set; }

    public decimal Price { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
