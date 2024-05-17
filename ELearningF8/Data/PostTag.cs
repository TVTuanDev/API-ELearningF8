using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class PostTag
{
    public int IdPost { get; set; }

    public int IdTag { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual Tag IdTagNavigation { get; set; } = null!;
}
