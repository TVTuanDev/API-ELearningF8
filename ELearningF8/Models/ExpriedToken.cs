namespace ELearningF8.Models
{
    public class ExpriedToken
    {
        public ExpriedToken()
        {
            Access = DateTime.UtcNow.AddHours(1);
            Refresh = DateTime.UtcNow.AddHours(12);
        }

        public DateTime Access {  get; set; } 
        public DateTime Refresh {  get; set; }

    }
}
