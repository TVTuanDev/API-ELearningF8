using System;
using System.Collections.Generic;

namespace ELearningF8.Data;

public partial class CourseRevenue
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public DateTime CreateAt { get; set; }

    public int IdCourse { get; set; }

    public int IdUser { get; set; }

    public virtual Course IdCourseNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
