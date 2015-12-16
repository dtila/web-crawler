using MovieCrawler.ApplicationServices;
using MovieCrawler.ApplicationServices.Contracts;
using MovieCrawler.Core;
using MovieCrawler.Domain;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MovieCrawler.Domain.Data;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    public class DivXOnlineMovieProvider : IMovieProvider
    {
        const string PageFormat = "http://www.divxonline.ro/page/{0}/";

        private static readonly Regex MovieIdRegex = new Regex("");
        private static readonly Regex MovieInfoRegex = new Regex("categoria:\\s+(.+)\\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Name { get { return "divxonline.ro"; } }
        
        public InspectMethodType GetInspectMethod(Uri uri)
        {
            return InspectMethodType.None;
        }

        public IPageSet EnumerateFromPage(int startPage)
        {
            return new Enumerator(startPage);
        }

        public Task AddToBuilder(MovieBuilder builder, BasicMovieInfo movie)
        {
            var movieInfo = movie as SummaryMovieInfo;
            if (movieInfo == null)
                throw new InvalidOperationException("");

            return GetMovieInfoAsync(builder, movie.Link);
        }

        public void AppendTo(MovieBuilder builder, BrowserPageInspectSubscription subscription)
        {
            throw new NotImplementedException();
        }

        private async Task GetMovieInfoAsync(MovieBuilder builder, Uri uri)
        {
            var html = await WebHttp.GetHtmlDocument(uri);

            var coverImage = html.DocumentNode.SelectSingleNode("//div[@class='single_port']/img")
                                                    .ThrowExceptionIfCoverImageNotExists()
                                                    .GetAttributeValue("src", null);
            var descriptionInput = HttpUtility.HtmlDecode(html.DocumentNode.SelectSingleNode("//div[@class='single_entry']")
                                                                            .ThrowExceptionIfMovieInfoNotExists().InnerText);
            var movieInfo = builder.MovieInfo;
            using (var sr = new StringReader(descriptionInput))
            {
                for (int i=0 ; i<3; i++)
                    sr.ReadLine();
                movieInfo.Description = sr.ReadToEnd();
            }

            var regexInput = HttpUtility.HtmlDecode(html.DocumentNode.SelectSingleNode("//div[@class='single_date_box']")
                                                                     .ThrowExceptionIfMovieInfoNotExists().InnerText);
            var match = MovieInfoRegex.Match(regexInput);
            if (match.Success)
                movieInfo.LoadGenresFrom(match.Groups[1].Value);

            foreach (var video in html.DocumentNode.SelectNodes("//div[@class='block-container']/center/div/iframe")
                                                   .ThrowExceptionIfNotExists("Unable to find the movie root"))
            {
                builder.Enqueue(this, HtmlHelpers.GetEmbededUri(video));
            }
        }

        private static ICollection<BasicMovieInfo> ParsePage(HtmlAgilityPack.HtmlDocument html)
        {
            var moviesCollection = new List<BasicMovieInfo>();
            var movies = html.DocumentNode.SelectNodes("//div[@class='movie-thumbnail']").ThrowExceptionIfNotExists("Unable to find movie root element");
            foreach(var movieDiv in movies)
            {
                var aelement = movieDiv.Element("a").ThrowExceptionIfNotExists("Unable to find a element");
                var img = movieDiv.SelectSingleNode("a/img").ThrowExceptionIfNotExists("Unable to find a/img element");

                var link = aelement.GetAttributeValue("href", null);
                var title = img.GetAttributeValue("alt", null).Substring(7); // lenght of (poster )
                moviesCollection.Add(new SummaryMovieInfo(title, new Uri(link)));
            }

            return moviesCollection;
        }


        class SummaryMovieInfo : BasicMovieInfo
        {
            public SummaryMovieInfo(string title, Uri uri) 
                : base(title, uri)
            {
            }
        }


        class Enumerator : MovieCrawler.Domain.Collections.SyncronizedEnumerator
        {
            public Enumerator(int? startPage)
                : base(startPage)
            {
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParsePage(int page)
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageFormat, CurrentPage)));
                return DivXOnlineMovieProvider.ParsePage(html);
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParseFirstPage()
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageFormat, CurrentPage)));

                var pages = html.DocumentNode.SelectSingleNode("//div[@class='wp-pagenavi']/span/text()")
                    .ThrowExceptionIfNotExists("Pages root element");

                var match = SharedRegex.EnglishPageMatchRegex.Match(pages.InnerText);
                if (match.Success)
                {
                    SetTotalPages(int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
                }

                return DivXOnlineMovieProvider.ParsePage(html);
            }
        }
    }
}
