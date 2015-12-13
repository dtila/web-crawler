using MovieCrawler.ApplicationServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.LinkScramblers
{
    class AdFlyLinkScrambler : ILinkScrambler
    {
        private Uri uri;
        public Uri Uri { get; private set; }
        
        public AdFlyLinkScrambler(Uri uri)
        {
            this.uri = uri;
        }

        public Task<Uri> GetLinkAsync()
        {
            return null;
        }

        /*
        public Task<MovieInfo> GetMovieInfoAsync(Uri uri)
        {
            var tcs = new TaskCompletionSource<MovieInfo>();
            var ie = new SHDocVw.InternetExplorer();
            ie.Visible = true;
            ie.DocumentComplete += (object pDisp, ref object URL) =>
            {
                ThreadPool.QueueUserWorkItem(PageDocumentComplete, new DocumentCompleteState
                {
                    Browser = ie,
                    TaskCompletionSource = tcs
                });
            };
            ie.Navigate(uri.ToString());
            return tcs.Task;
        }

        private async void PageDocumentComplete(object state)
        {
            var requestState = state as DocumentCompleteState;

            try
            {
                var url = await GetHostedLinkAsync(requestState.Browser.Document as mshtml.HTMLDocument);
                // adf.ly does not hosts movies, but instead it hosts another link, so we need to forward the link and the result
                var crawler = CrawlingFactory.CreateMovieCrawler(url);
                crawler.ProxyCreateDelegate = this.ProxyCreateDelegate;
                requestState.TaskCompletionSource.SetResult(await crawler.GetMovieInfoAsync(url));
            }
            catch (Exception ex)
            {
                requestState.TaskCompletionSource.SetException(ex);
            }
            finally
            {
                requestState.Browser.Quit();
            }
        }

        private async Task<Uri> GetHostedLinkAsync(mshtml.HTMLDocument document)
        {
            for (int i = 0; i < 10; i++, await Task.Delay(1000)) //max number of retries: 10 seconds
            {
                var elem = document.getElementById("skip_button");
                if (elem != null)
                {
                    var href = elem.getAttribute("href") as string;
                    if (!string.IsNullOrEmpty(href))
                        return new Uri(href);
                }
            }

            throw new InvalidDOMStructureException("Unable to find the 'skip_button' element");
        }


        class DocumentCompleteState
        {
            public SHDocVw.InternetExplorer Browser { get; set; }
            public TaskCompletionSource<MovieInfo> TaskCompletionSource { get; set; }
        }*/
    }
}
