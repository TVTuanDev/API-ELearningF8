using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class UserCourse
{
    public int IdUser { get; set; }

    public int IdCourse { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Course IdCourseNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
