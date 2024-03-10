namespace ELearningF8.Models
{
    public class ExpriedToken
    {
        public ExpriedToken()
        {
            Access = DateTime.UtcNow.AddMinutes(2);
            Refresh = DateTime.UtcNow.AddHours(1);
        }

        public DateTime Access {  get; set; } 
        public DateTime Refresh {  get; set; }

    }
}
