using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Video
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Avatar { get; set; } = null!;

    public string Link { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }
}
