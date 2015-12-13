using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MovieCrawler.ApplicationServices
{
    static class Utilities
    {
        public static string EscapeJavaScript(string input)
        {
            const string unescape = "unescape(";
            input = HttpUtility.UrlDecode(input);
            int idx = input.IndexOf(unescape, StringComparison.CurrentCultureIgnoreCase);

            if (idx > 0)
            {
                var start = idx + unescape.Length;
                while (input[start] != '\"')
                    start++;
                int end = input.IndexOf(')', start + 1);
                if (end < 0)
                    throw new InvalidOperationException("Unable to unescape because the end string delimiter was not found");

                return input.Substring(0, idx) + '\"' + HttpUtility.JavaScriptStringEncode(EscapeJavaScript(input.Substring(start + 1, end - start - 1 - 1))) + '\"' + input.Substring(end + 1);
            }

            return input;
        }
    }
}
