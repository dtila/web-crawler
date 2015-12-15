using MovieCrawler.ApplicationServices.MovieProviders;
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
        }

        [STAThread]
        static void Main(string[] args)
        {
            TestProvider().Wait();
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
    }
}
