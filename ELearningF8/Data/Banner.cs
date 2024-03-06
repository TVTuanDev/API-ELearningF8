using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Banner
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Descriptions { get; set; }

    public string? Slug { get; set; }

    public string? Img { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }
}
