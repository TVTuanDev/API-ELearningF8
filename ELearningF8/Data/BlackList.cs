using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class BlackList
{
    public int Id { get; set; }

    public string AccessToken { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }
}
