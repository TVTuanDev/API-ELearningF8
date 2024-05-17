using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text;

namespace ELearningF8.Models
{
    public class UserModel
    {
        private readonly IMemoryCache _cache;

        public UserModel(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
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

        //public static bool VerifyEmail(string email, string code)
        //{

        //    // Check email và code
        //    var getCache = _cache.Get<string>(email);
        //    if (getCache == code) return true;
        //    return false;
        //}
    }
}
