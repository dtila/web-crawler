using MovieCrawler.Domain;
using MovieCrawler.Domain.Builder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices
{
    static class HttpExtensions
    {
        public static HtmlAgilityPack.HtmlNode ThrowExceptionIfNotExists(this HtmlAgilityPack.HtmlNode element, String message)
        {
            if (element == null)
                throw new InvalidDOMStructureException(message);
            return element;
        }

        public static HtmlAgilityPack.HtmlNodeCollection ThrowExceptionIfNotExists(this HtmlAgilityPack.HtmlNodeCollection collection, string message)
        {
            if (collection == null)
                throw new InvalidDOMStructureException(message);
            return collection;
        }

        public static HtmlAgilityPack.HtmlNode ThrowExceptionIfCoverImageNotExists(this HtmlAgilityPack.HtmlNode element)
        {
            if (element == null)
                throw new InvalidDOMStructureException("Unable to find the cover image element");
            return element;
        }

        public static HtmlAgilityPack.HtmlNode ThrowExceptionIfDescriptionNotExists(this HtmlAgilityPack.HtmlNode element)
        {
            if (element == null)
                throw new InvalidDOMStructureException("Unable to find the movie description element");
            return element;
        }

        public static HtmlAgilityPack.HtmlNode ThrowExceptionIfTitleNotExists(this HtmlAgilityPack.HtmlNode element)
        {
            if (element == null)
                throw new InvalidDOMStructureException("Unable to find the title root element");
            return element;
        }

        public static HtmlAgilityPack.HtmlNode ThrowExceptionIfMovieInfoNotExists(this HtmlAgilityPack.HtmlNode element)
        {
            if (element == null)
                throw new InvalidDOMStructureException("Unable to find an element necessary for the movie info");
            return element;
        }
    }
}
