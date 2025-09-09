using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ASOFT.A00.Business.Helpers
{
    public class TextHelper
    {
        public static string RemoveBlankAndUnicode(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            string stFormD = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            var result = sb.ToString().Normalize(NormalizationForm.FormC);

            return result.Replace(" ", "").ToLower();
        }
    }
}
