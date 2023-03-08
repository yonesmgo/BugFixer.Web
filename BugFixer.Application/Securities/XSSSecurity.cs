using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Securities
{
    public static class XSSSecurity
    {
        public static string SanitizeText(this string text)
        {
            var sanitizedText = new HtmlSanitizer()
            {
                AllowedTags = { "p" },
                AllowDataAttributes = true
            };
            return sanitizedText.Sanitize(text);
        }
    }
}
