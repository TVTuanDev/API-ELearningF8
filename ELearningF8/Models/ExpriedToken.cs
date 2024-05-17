namespace ELearningF8.Models
{
    public class ExpriedToken
    {
        public static DateTime Access {  get; } = DateTime.UtcNow.AddHours(1);
        public static DateTime Refresh {  get; } = DateTime.UtcNow.AddDays(1);

    }
}
