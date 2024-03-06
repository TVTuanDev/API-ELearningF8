using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
