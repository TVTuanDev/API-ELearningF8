using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Comment
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdPost { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? ParentId { get; set; }

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment? Parent { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
