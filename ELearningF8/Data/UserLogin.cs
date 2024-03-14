using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class UserLogin
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
