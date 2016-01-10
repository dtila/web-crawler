using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;
using IBrowser = WebCrawler.Browser.IBrowser;

namespace MovieCrawler.ConsoleTester.Infrastructure.Chromium
{
    class ChroumiumBrowserFactory : IBrowserFactory
    {
        static ChroumiumBrowserFactory()
        {
            Cef.Initialize(new CefSettings() { MultiThreadedMessageLoop = true, WindowlessRenderingEnabled = true });
        }

        public IBrowser Create()
        {
            return new ChromiumBrowser();
        }
    }
}
