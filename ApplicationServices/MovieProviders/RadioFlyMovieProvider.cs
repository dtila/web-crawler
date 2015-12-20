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
using WebCrawler.Data;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    class RadioFlyMovieProvider : IMovieProvider
    {
        private static readonly string PageAddress = "http://www.radiofly.ws/filme-online-gratis.php";

        public string Name
        {
            get { return "RadioFly"; }
        }

        public void AppendTo(IContentBuilder builder)
        {
            throw new NotImplementedException();
        }

        public Task AddToBuilder(MovieBuilder builder, BasicMovieInfo movie)
        {
            throw new NotImplementedException();
        }

        public IPageSet EnumerateFromPage(int page)
        {
            throw new NotImplementedException();
        }
    }
}
