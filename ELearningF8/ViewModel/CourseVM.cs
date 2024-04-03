namespace ELearningF8.ViewModel
{
    public class ListCourses
    {
        public List<CourseVM> Courses { get; set; } = null!;
    }

    public class CourseVM
    {
        public string Title { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string? Descriptions { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        List<ChapterView>? Chapters { get; set; }
    }

    public class ChapterView
    {
        public string Title { get; set; } = null!;
        public int Sort { get; set; }
        public List<LessonView>? Lessons { get; set; }
    }

    public class LessonView
    {
        public string Title { get; set; } = null!;
        public int Sort { get; set; }
        public string? Content { get; set; }
        public string? Link { get; set; }
        public string? Slug { get; set; }
    }
}
