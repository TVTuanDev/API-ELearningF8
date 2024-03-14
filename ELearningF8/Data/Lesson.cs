using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Lesson
{
    public int Id { get; set; }

    public int IdChapter { get; set; }

    public string Title { get; set; } = null!;

    public string? Descriptions { get; set; }

    public string? Content { get; set; }

    public int Sort { get; set; }

    public string? LinkImg { get; set; }

    public string? LinkVideo { get; set; }

    public string? Link { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Chapter IdChapterNavigation { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
