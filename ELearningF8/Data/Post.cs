﻿using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string? Descriptions { get; set; }

    public string? Content { get; set; }

    public string? Slug { get; set; }

    public bool IsPublish { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int IdUser { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
