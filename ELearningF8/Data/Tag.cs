using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class Tag
{
    public int Id { get; set; }

    public string TagName { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
