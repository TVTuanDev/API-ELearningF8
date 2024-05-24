using ELearningF8.Utilities;
using System.Globalization;
using System.Text;

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

        public static string RemoveDiacriticsAndSpaces(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c != ' ')
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
