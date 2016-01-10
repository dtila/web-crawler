using MovieCrawler.ApplicationServices.MovieProviders;
using MovieCrawler.Core;
using MovieCrawler.Domain;
using MovieCrawler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCrawler;
using WebCrawler.Browser;
using WebCrawler.Browser.Chromium;
using WebCrawler.Infrastructure;

namespace MovieHtmlParser
{
    class Program
    {
        static Program()
        {
            DependencyResolver.Current = new Infrastructure.SimpleInjectorDependencyResolver();
            DependencyResolver.Register<IHttpFactory>(new Infrastructure.HttpFactory());
            DependencyResolver.Register<ILoggerFactory>(new Infrastructure.LoggerFactory());
            DependencyResolver.Register<IMoviesRepository>(new Infrastructure.MoviesRepository());

            DependencyResolver.Register<IMovieProvider>(new FilmeOnline2013MovieProvider());

            DependencyResolver.Register<IBrowserFactory>(new ChromiumBrowserFactory());
            //DependencyResolver.Register<IBrowserFactory>(new Infrastructure.BrowserFactory());

            MovieCrawler.ApplicationServices.ApplicationServices.Init();
        }

        [STAThread]
        static void Main(string[] args)
        {
            var browser = new ChromiumBrowserFactory().Create();
            browser.Navigate(new Uri("http://google.ro")).Wait();
            var x = browser.Page.Root.Query("#hplogo").GetAttribute("align");

            //Download().Wait();
            //TestProvider().Wait();
            Console.ReadLine();
        }

        static async Task TestProvider()
        {
            var provider = new FilmeOnline2013MovieProvider();
            var enumerator = provider.EnumerateFromPage(1);

            foreach (var pageTask in enumerator)
            {
                var page = await pageTask;
                foreach (var item in page)
                {
                    Debug.WriteLine(item.Title);       
                }
            }
        }

        static async Task Download()
        {
            var service = DependencyResolver.Resolve<CrawlerService>();
            var movieProvider = new MovieCrawler.Domain.Model.MovieProvider("Empty");
            await service.ParseProviderNewMovies(movieProvider);
        }
    }
}
