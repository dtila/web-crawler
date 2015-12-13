using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices
{
    class HtmlHelpers
    {
        public static Uri GetEmbededUri(HtmlAgilityPack.HtmlNode movieElement)
        {
            var type = movieElement.Name.ToLower();

            switch (type)
            {
                case "iframe": return new Uri(movieElement.GetAttributeValue("src", null));
                case "object":
                    {
                        if (!movieElement.GetAttributeValue("type", "").Equals("application/x-shockwave-flash", StringComparison.CurrentCultureIgnoreCase))
                            throw new InvalidOperationException("unable to create a StreamCrawler for an embedded object which is not flash!");
                        var flashVars = movieElement.GetAttributeValue("flashvars", null);
                        return null;
                    }
                default:
                    throw new NotImplementedException(string.Format("Unable to retrieve a stream crawler for the element type {0}", type));
            }
        }
    }
}
