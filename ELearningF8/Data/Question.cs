﻿using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Question
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdLesson { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? ParentId { get; set; }

    public virtual Lesson IdLessonNavigation { get; set; } = null!;

    public virtual Comment? Parent { get; set; }
}
