using MovieCrawler.Domain;
using MovieCrawler.Domain.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Browser;
using WebCrawler.Content.Builder;
using WebCrawler.Data;

namespace MovieCrawler.ApplicationServices.LinkScramblers
{
    class AdFlyLinkScrambler : ILinkUnscrambler
    {
        private Uri uri;
        private IContentBuilder builder;
        private TaskCompletionSource<Uri> taskCompletionSource;
        
        public AdFlyLinkScrambler(Uri uri)
        {
            this.uri = uri;
            taskCompletionSource = new TaskCompletionSource<Uri>();
        }

        public Task<Uri> GetLinkAsync()
        {
            if (builder == null)
                throw new InvalidOperationException("AdFly scrambler must be added to a content builder before obtaining a link");
            builder.Enqueue(new DelegatedBrowserContentRequest(uri, GetHostedLinkAsync));
            return taskCompletionSource.Task;
        }

        public void AppendTo(IContentBuilder builder)
        {
            this.builder = builder;
        }

        private async Task GetHostedLinkAsync(IBrowserPageInspectSubscription subscription)
        {
            for (int i = 0; i < 10; i++, await Task.Delay(1000)) //max number of retries: 10 seconds
            {
                var elem = subscription.Page.Root.Query("#skip_button");
                if (elem != null)
                {
                    var href = elem.GetAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        taskCompletionSource.SetResult(new Uri(href));
                        return;
                    }
                }
            }

            taskCompletionSource.SetException(new InvalidDOMStructureException("Unable to find the 'skip_button' element"));
        }
    }
}
