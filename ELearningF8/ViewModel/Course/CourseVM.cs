using ELearningF8.Models;

namespace ELearningF8.ViewModel.Course
{
    public class CourseVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Avatar { get; set; } = DefaultModel.Image;
        public string? Descriptions { get; set; }
        public string? Content { get; set; }
        public string TypeCourse { get; set; } = "free";
        public decimal Price { get; set; } = 0;
        public decimal? Discount { get; set; }
        public bool IsComing { get; set; }
        public bool IsPublish { get; set; }
    }

    public class ChapterVM
    {
        public string Title { get; set; } = null!;
        public int Sort { get; set; }
    }

    public class LessonVM
    {
        public string Title { get; set; } = null!;
        public int Sort { get; set; }
        public string? Content { get; set; }
        public string? Link { get; set; }
        //public string? Slug { get; set; }
    }
}
