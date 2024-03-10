using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string JwtId { get; set; } = null!;

    public string Token { get; set; } = null!;

    public bool? IsUsed { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
