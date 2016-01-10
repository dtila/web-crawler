using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;
using CefSharp;
using IBrowser = WebCrawler.Browser.IBrowser;
using System.Threading;

namespace MovieCrawler.ConsoleTester.Infrastructure.Chromium
{
    class ChromiumBrowser : IBrowser
    {
        private CefSharp.OffScreen.ChromiumWebBrowser browser;
        private ManualResetEventSlim initializeEvent;
        private IBrowserPage page;

        public ChromiumBrowser()
        {
            browser = new CefSharp.OffScreen.ChromiumWebBrowser();
            initializeEvent = new ManualResetEventSlim(false);

            browser.BrowserInitialized += Browser_BrowserInitialized;
            initializeEvent.Wait();
        }

        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            browser.BrowserInitialized -= Browser_BrowserInitialized;
            initializeEvent.Set();
            page = new ChromiumPage(browser);
        }

        public IBrowserPage Page
        {
            get
            {
                initializeEvent.Wait();
                return page;
            }
        }

        public Uri Uri
        {
            get { return new Uri(browser.Address); }
        }

        public Task Navigate(Uri uri)
        {
            return LoadPageAsync(browser, uri.ToString());
        }

        private static Task LoadPageAsync(IWebBrowser browser, string address = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            browser.LoadingStateChanged += handler;
            if (!string.IsNullOrEmpty(address))
                browser.Load(address);
            return tcs.Task;
        }

    }
}
