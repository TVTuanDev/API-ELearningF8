namespace ELearningF8.Models
{
    public class JsonResult
    {
        public int Status { get; set; }
        public string Message { get; set; } = null!;

        public string[]? Data { get; set; }
    }
}
