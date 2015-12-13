using MovieCrawler.ApplicationServices;
using MovieCrawler.ApplicationServices.Contracts;
using MovieCrawler.Core;
using MovieCrawler.Domain;
using MovieCrawler.Domain.Collections;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MovieCrawler.Domain.Data;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    public class BratuMarianMovieProvider : IMovieProvider 
    {
        private static readonly string PageFormat = "http://bratu-marian.ro/filmenoi/?paged={0}";

        public string Name { get { return "BratuMarian.ro"; } }
        public Uri Uri { get; private set; }

        public BratuMarianMovieProvider()
        {
        }

        public IPageSet EnumerateFromPage(int startPage)
        {
            return new Enumerator(startPage);
        }

        public Task AddToBuilder(MovieBuilder builder, BasicMovieInfo movie)
        {
            var movieInfo = movie as SummaryMovieInfo;
            if (movieInfo == null)
                throw new InvalidOperationException("Unknown movie info");

            return ParseMovieInfoAsync(builder, movie.Link);
        }

        public void AppendTo(MovieBuilder builder, PageInspectSubscription subscription)
        {
            throw new NotImplementedException();
        }

        private static async Task ParseMovieInfoAsync(MovieBuilder builder, Uri uri)
        {
            var html = await WebHttp.GetHtmlDocument(uri);

            var movieRootElement = html.DocumentNode.SelectSingleNode("//div[@class='filmcontent']/div[@class='filmalti']")
                                       .ThrowExceptionIfNotExists("Unable to find the movie root element");

            var imgElement = movieRootElement.SelectSingleNode("div[@class='filmaltiimg']/img").ThrowExceptionIfNotExists("Unable to find the 'img' tag element");
            var movieDetailsElement = movieRootElement.SelectSingleNode("div[@class='filmaltiaciklama']")
                                                      .ThrowExceptionIfNotExists("Unable to find the movie details element");
            var categories = movieDetailsElement.SelectNodes("p[1]/a/text()")
                                                .ThrowExceptionIfNotExists("Unable to find the categories elements")
                                                .Select(li => HttpUtility.HtmlDecode(li.InnerText));

            var movieInfo = new MovieInfo(new BasicMovieInfo(imgElement.GetAttributeValue("alt", null), uri));
            movieInfo.CoverImage = new Uri(imgElement.GetAttributeValue("src", null));

            var description = movieDetailsElement.SelectSingleNode("//p[last()]").ThrowExceptionIfNotExists("Unable to find the movie description element").InnerText;
            int idx = description.IndexOf(':');
            if (idx > 0)
                movieInfo.Description = HttpUtility.HtmlDecode(description.Substring(idx + 1).Trim());

            foreach (var embeddedMovies in html.DocumentNode.SelectNodes("//object[@type='application/x-shockwave-flash']/param[@name='flashvars']"))
            {
                var query = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(embeddedMovies.GetAttributeValue("value", null)));
                
                string captionFile = query.Get("captions.file");
                var avstream = HttpUtility.UrlDecode(query.Get("streamer"));

                if (string.IsNullOrEmpty(captionFile) || string.IsNullOrEmpty(avstream))
                    continue;

                var streamSet = new MovieStream();
                streamSet.Captions.Add(new Caption
                {
                    Language = "Romanian",
                    Address = captionFile
                });

                streamSet.VideoStreams.Add(new VideoStream { AVStream = avstream });
                movieInfo.Streams.Add(streamSet);
                builder.AddStream(streamSet);
            }
        }

        private static ICollection<BasicMovieInfo> ParsePage(HtmlAgilityPack.HtmlDocument html)
        {
            var movies = new List<BasicMovieInfo>();

            foreach (var film in html.DocumentNode.SelectNodes("//div[@class='moviefilm']/a"))
            {
                var link = film.GetAttributeValue("href", null);

                var titleElement = film.SelectSingleNode("img");
                if (titleElement == null)
                    throw new InvalidParseElementException("img");

                var title = HttpUtility.HtmlDecode(titleElement.GetAttributeValue("alt", null));
                if (title == null)
                    throw new InvalidParseElementException("alt attribute");

                movies.Add(new SummaryMovieInfo(title, new Uri(link)));
            }

            return movies;
        }



        class SummaryMovieInfo : BasicMovieInfo
        {
            public SummaryMovieInfo(string title, Uri uri) : base(title, uri)
            {
            }
        }

        class Enumerator : SyncronizedEnumerator
        {
            public Enumerator(int? startPage) 
                : base(startPage)
            {
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParsePage(int page)
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageFormat, CurrentPage)));
                return BratuMarianMovieProvider.ParsePage(html);
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParseFirstPage()
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageFormat, 1)));

                var pages = html.DocumentNode.SelectSingleNode("//div[@class='wp-pagenavi']/span/text()")
                                             .ThrowExceptionIfNotExists("Unable to find the page number section");
                var match = SharedRegex.EnglishPageMatchRegex.Match(pages.InnerText);
                if (match.Success)
                {
                    SetTotalPages(int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
                }

                return BratuMarianMovieProvider.ParsePage(html);
            }
        }
    }
}