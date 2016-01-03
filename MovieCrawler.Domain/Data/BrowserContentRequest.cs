using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data;

namespace MovieCrawler.Domain.Data
{
    public delegate Task BrowserContentRequestCallback(IBrowserPageInspectSubscription subsciption);

    public class BrowserContentRequest : IBrowserContent
    {
        private BrowserContentRequestCallback pageSubscription;

        public Uri Uri { get; private set; }

        public BrowserContentRequest(Uri uri, BrowserContentRequestCallback callback)
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
