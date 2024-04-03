using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class TypeLesson
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
