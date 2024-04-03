namespace ELearningF8.ViewModel
{
    public class BannerVM
    {
        public string Title { get; set; } = null!;

        public string? Descriptions { get; set; }

        public string? Link { get; set; }

        public IFormFile fileImg { get; set; } = null!;
    }
}
