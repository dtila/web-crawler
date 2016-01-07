using MovieCrawler.ApplicationServices.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Content.Builder;
using WebCrawler.Infrastructure;

namespace MovieCrawler.ApplicationServices
{
    public class ApplicationServices
    {
        public static void Init()
        {
            DependencyResolver.Register<ICrawlerFactory>(new CrawlerFactory());
        }
    }
}
