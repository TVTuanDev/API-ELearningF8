using ELearningF8.Utilities;

namespace ELearningF8.Models
{
    public class ConvertModel
    {
        public static string ConvertSlug(string title)
        {
            if (string.IsNullOrEmpty(title)) return string.Empty;

            var slug = AppUtilities.GenerateSlug(title) + ".html";

            return slug;
        }
    }
}
