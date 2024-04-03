using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Answer
{
    public int Id { get; set; }

    public string AnswerQuestion { get; set; } = null!;

    public bool Status { get; set; }

    public string Explain { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int IdQuestion { get; set; }

    public virtual QuestionLesson IdQuestionNavigation { get; set; } = null!;
}
