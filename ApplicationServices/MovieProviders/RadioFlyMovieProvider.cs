using MovieCrawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MovieCrawler.Domain.Data;
using MovieCrawler.Domain.Model;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    class RadioFlyMovieProvider : IMovieProvider
    {
        private static readonly string PageAddress = "http://www.radiofly.ws/filme-online-gratis.php";

        public string Name
        {
            get { return "RadioFly"; }
        }

        public Task AddToBuilder(MovieBuilder builder, BasicMovieInfo movie)
        {
            throw new NotImplementedException();
        }

        public void AppendTo(MovieBuilder builder, BrowserPageInspectSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public IPageSet EnumerateFromPage(int page)
        {
            throw new NotImplementedException();
        }

        public InspectMethodType GetInspectMethod(Uri uri)
        {
            return InspectMethodType.Raw;
        }
    }
}
