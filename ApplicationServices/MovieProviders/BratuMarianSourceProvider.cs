using Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlingLibrary.MovieProviders
{
    public class BratuMarianSourceProvider : IMovieSourceProvider
    {
        private static readonly string PageFormat = "http://bratu-marian.ro/filmenoi/?paged={0}";
        private static readonly Regex _pageMatchRegex = new Regex("Page (\\d+) of (\\d+)", RegexOptions.Compiled);
        private int _currentPageIndex;

        public BratuMarianSourceProvider()
        {
            _currentPageIndex = 0;
        }

        public string Name { get { return "BratuMarian.ro"; } }

        public bool IsFinished { get; private set; }

        public Func<IWebProxy> ProxyFactoryDelegate { get; set; }

        public async Task<ICollection<Commons.MovieInfo>> GetMoviesAsync()
        {
            for (int page = 1; ; page++)
            {
                var httpRequest = WebRequest.CreateHttp(string.Format(PageFormat, ++_currentPageIndex));
                if (ProxyFactoryDelegate != null)
                    httpRequest.Proxy = ProxyFactoryDelegate();
                var response = await httpRequest.GetResponseAsync();
                var html = new HtmlAgilityPack.HtmlDocument();
                html.Load(response.GetResponseStream());

                foreach (var film in html.DocumentNode.SelectNodes("//div[@class='moviefilm']/a"))
                {
                    var link = film.GetAttributeValue("href", null);
                }

                var pages = html.DocumentNode.SelectSingleNode("//div[@class='wp-pagenavi']/span/text()");
                var match = _pageMatchRegex.Match(pages.InnerText);
                if (match.Success)
                {
                    var currentPage = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var maxPage = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    this.IsFinished = currentPage == maxPage;
                }


                return null;
            }
        }

        private void dd()
        {
        }

        void ie_DocumentComplete(object pDisp, ref object URL)
        {
            var ie = pDisp as SHDocVw.InternetExplorer;

        }


        
    }
}
