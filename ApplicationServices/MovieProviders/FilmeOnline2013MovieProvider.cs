using MovieCrawler.ApplicationServices;
using MovieCrawler.Core;
using MovieCrawler.Domain;
using MovieCrawler.Domain.Collections;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MovieCrawler.Domain.Data;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    public class FilmeOnline2013MovieProvider : IMovieProvider
    {
        private const string PageUriFormat = "http://www.filmeonline2013.biz/page/{0}/";

        public string Name { get { return "TopFilme2013.net"; } }


        public FilmeOnline2013MovieProvider()
        {
        }

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
                throw new ArgumentException("");

            return Build(builder, movie.Link);
        }

        public void AppendTo(MovieBuilder builder, BrowserPageInspectSubscription subscription)
        {
            throw new NotImplementedException();
        }

        private async Task Build(MovieBuilder builder, Uri uri)
        {
            var html = await WebHttp.GetHtmlDocument(uri);

            var entryContent = html.DocumentNode.SelectSingleNode("//div[@class='entry entry-content']")
                                                .ThrowExceptionIfNotExists("Unable to find the movie details element");
            var img = entryContent.SelectSingleNode("a[@class='entry-thumb']/img").ThrowExceptionIfNotExists("Movie thumbnail element");
            var categories = entryContent.SelectNodes("a[@rel='category tag']/text()")
                                         .ThrowExceptionIfNotExists("Unable to find the categories element")
                                         .Select(li => HttpUtility.HtmlDecode(li.InnerText));

            var movieInfo = builder.MovieInfo;
            movieInfo.LoadGenresFrom(categories);
            movieInfo.CoverImage = new Uri(img.GetAttributeValue("src", null));

            var description = entryContent.SelectSingleNode("p").ThrowExceptionIfNotExists("Unable to find description element");
            movieInfo.Description = HttpUtility.HtmlDecode(description.InnerText);
            
            foreach (var entry in html.DocumentNode.SelectNodes("//div[@class='entry-embed']/iframe")
                            .ThrowExceptionIfNotExists("Unable to find the movie streams"))
            {
                builder.Enqueue(HtmlHelpers.GetEmbededUri(entry));
            }
        }


        class SummaryMovieInfo : BasicMovieInfo
        {
            public Uri CoverUri;

            public SummaryMovieInfo(string title, Uri uri, Uri coverUri) : base(title, uri)
            {
                this.CoverUri = uri;
            }
        }

        class Enumerator : SyncronizedEnumerator
        {
            public Enumerator(int? startPage) 
                : base(startPage)
            {
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParseFirstPage()
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageUriFormat, CurrentPage)));

                var movies = GetTopMovies(html);
                foreach (var movie in GetPaggedMovies(html))
                    movies.Add(movie);

                var pagesElement = html.DocumentNode.SelectSingleNode("//div[@class='wp-pagenavi']/span")
                                                            .ThrowExceptionIfNotExists("Unable to find the pages number element");

                var pagesMatch = SharedRegex.EnglishPageMatchRegex.Match(pagesElement.InnerText);
                if (!pagesMatch.Success)
                    throw new InvalidParseElementException("Unable to determine the pages count");

                SetTotalPages(int.Parse(pagesMatch.Groups[2].Value));
                return movies;
            }

            protected override async Task<ICollection<BasicMovieInfo>> ParsePage(int page)
            {
                var html = await WebHttp.GetHtmlDocument(new Uri(string.Format(PageUriFormat, CurrentPage)));
                return GetPaggedMovies(html);
            }

            private ICollection<BasicMovieInfo> GetTopMovies(HtmlAgilityPack.HtmlDocument html)
            {
                var list = new List<BasicMovieInfo>();
                foreach (var movie in html.DocumentNode.SelectNodes("//div[@class='smooth_slideri']/a"))
                {
                    var link = movie.GetAttributeValue("href", null);
                    var title = movie.GetAttributeValue("title", null);
                    var img = movie.SelectSingleNode("img[@src]").ThrowExceptionIfNotExists("Movie cover not found").InnerText;

                    list.Add(new SummaryMovieInfo(title, new Uri(link), new Uri(img)));
                }
                return list;
            }

            private ICollection<BasicMovieInfo> GetPaggedMovies(HtmlAgilityPack.HtmlDocument html)
            {
                var list = new List<BasicMovieInfo>();
                foreach (var movie in html.DocumentNode.SelectNodes("//a[@class='entry-thumb2']"))
                {
                    var link = movie.GetAttributeValue("href", null);
                    var title = movie.GetAttributeValue("title", null);

                    list.Add(new SummaryMovieInfo(title, new Uri(link), null));
                }
                return list;
            }
        }
    }
}
