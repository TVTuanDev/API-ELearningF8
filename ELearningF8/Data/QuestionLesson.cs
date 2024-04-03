using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class QuestionLesson
{
    public int Id { get; set; }

    public string? Descriptions { get; set; }

    public string Question { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int IdLesson { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual Lesson IdLessonNavigation { get; set; } = null!;
}
