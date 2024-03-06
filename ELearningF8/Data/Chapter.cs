using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Chapter
{
    public int Id { get; set; }

    public int IdCourse { get; set; }

    public string Title { get; set; } = null!;

    public int Sort { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Course IdCourseNavigation { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
