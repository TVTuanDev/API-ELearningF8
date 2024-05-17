namespace ELearningF8.ViewModel.Post
{
    public class PostVM
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Avatar { get; set; }

        public string? Descriptions { get; set; }

        public string? Content { get; set; }

        public bool IsPublish { get; set; } = false;

        public string? Tag { get; set; }
    }
}
