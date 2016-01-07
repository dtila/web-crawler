using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Content;
using WebCrawler.Data;

namespace WebCrawler.Browser
{
    public delegate Task BrowserContentRequestCallback(IBrowserPageInspectSubscription subsciption);

    public class DelegatedBrowserContentRequest : IBrowserContent
    {
        private BrowserContentRequestCallback pageSubscription;

        public Uri Uri { get; private set; }

        public DelegatedBrowserContentRequest(Uri uri, BrowserContentRequestCallback callback)
        {
            this.Uri = uri;
            this.pageSubscription = callback;
        }

        public async void Fullfill(IBrowserPageInspectSubscription subscription)
        {
            await pageSubscription(subscription);
        }
    }
}
