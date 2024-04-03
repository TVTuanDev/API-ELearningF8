using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Lesson
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Sort { get; set; }

    public string? Content { get; set; }

    public string? Link { get; set; }

    public string? Slug { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int IdChapter { get; set; }

    public int IdType { get; set; }

    public virtual Chapter IdChapterNavigation { get; set; } = null!;

    public virtual TypeLesson IdTypeNavigation { get; set; } = null!;

    public virtual ICollection<QuestionLesson> QuestionLessons { get; set; } = new List<QuestionLesson>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
