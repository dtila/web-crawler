using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;
using IBrowser = WebCrawler.Browser.IBrowser;

namespace WebCrawler.Browser.Chromium
{
    public class ChromiumBrowserFactory : IBrowserFactory
    {
        static ChromiumBrowserFactory()
        {
            Cef.Initialize(new CefSettings() { MultiThreadedMessageLoop = true, WindowlessRenderingEnabled = true });
        }

        public IBrowser Create()
        {
            return new ChromiumBrowser();
        }
    }
}
