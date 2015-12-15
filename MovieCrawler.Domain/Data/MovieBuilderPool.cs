using MovieCrawler.Domain.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Browser;
using WebCrawler.Logging;

namespace MovieCrawler.Domain.Data
{
    public class MovieBuilderPool
    {
        private const int MaxRunningPages = 2;

        private List<RunningPage> runningPages;
        private ConcurrentQueue<IBrowser> availableWorkers;
        private ConcurrentQueue<Page> enqueuedPages;
        private IBrowserFactory browserFactory;
        private ILogger logger;

        public MovieBuilderPool()
        {
            browserFactory = DependencyResolver.Resolve<IBrowserFactory>();
            logger = LoggerFactory.Create("MovieBuilderPool");

            availableWorkers = new ConcurrentQueue<IBrowser>();
            runningPages = new List<RunningPage>();
            enqueuedPages = new ConcurrentQueue<Page>();

            for (int i=0; i<MaxRunningPages; i++)
            {
                availableWorkers.Enqueue(browserFactory.Create());
            }
        }

        public MovieBuilder CreateBuilder(BasicMovieInfo movieInfo)
        {
            var builder = new MovieBuilder(this, movieInfo);
            return builder;
        }

        internal void Enqueue(Uri uri, MovieBuilder builder)
        {
            IBrowser browser;
            if (runningPages.Count < MaxRunningPages && availableWorkers.TryDequeue(out browser))
            {
                try
                {
                    lock(this)
                    {
                        runningPages.Add(new RunningPage(uri, builder, browser));
                    }
                    browser.Navigate(uri).ContinueWith(task => PageLoaded(new BrowserPageInspectSubscription(this, uri, browser, builder)));
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Unable to navigate to the page {0}", uri), ex);
                    availableWorkers.Enqueue(browser);
                    throw;
                }
            }

            logger.Info("An address has been enqueued: " + uri);
            enqueuedPages.Enqueue(new Page(uri, builder));
        }

        internal void FreePage(Uri uri, IBrowser browser)
        {
            var page = default(RunningPage);
            lock (this)
            {
                var idx = runningPages.FindIndex(li => li.Uri == uri);
                if (idx < 0)
                    throw new InvalidOperationException("The requested page '" + uri + "' was not found in the running pages");

                page = runningPages[idx];
                runningPages.RemoveAt(idx);
            }

            availableWorkers.Enqueue(browser);
        }

        private void PageLoaded(BrowserPageInspectSubscription pageInspectSubscription)
        {
            pageInspectSubscription.Inspect();

            Page enqueuedPage;
            if (enqueuedPages.TryDequeue(out enqueuedPage))
            {
                Enqueue(enqueuedPage.Uri, enqueuedPage.Builder);
            }
        }

        struct Page
        {
            public Uri Uri;
            public MovieBuilder Builder;

            public Page(Uri uri, MovieBuilder builder)
            {
                Uri = uri;
                Builder = builder;
            }

            public override int GetHashCode()
            {
                return Uri.GetHashCode();
            }
        }

        struct RunningPage
        {
            public Uri Uri;
            public MovieBuilder Builder;
            public IBrowser Browser;

            public RunningPage(Uri uri, MovieBuilder builder, IBrowser browser)
            {
                Uri = uri;
                Builder = builder;
                Browser = browser;
            }
        }
    }

    /// <summary>
    /// Represent an object that can be used to persist the inspection of a page, and to be disposed when is not needed
    /// </summary>
    public class BrowserPageInspectSubscription : IDisposable
    {
        private MovieBuilderPool pool;
        private IBrowser browser;
        private Uri originalUri;
        private MovieBuilder builder;

        public Uri OriginalUri { get { return originalUri; } }
        public Uri CurrentUri { get { return browser.Uri; } }
        public IBrowserPage Page { get { return browser.Page; } }

        internal BrowserPageInspectSubscription(MovieBuilderPool pool, Uri originalUri, IBrowser browser, MovieBuilder builder)
        {
            this.builder = builder;
            this.originalUri = originalUri;
            this.browser = browser;
            this.pool = pool;
        }

        internal void Inspect()
        {
            builder.OnPageLoaded(this);
        }

        public void Dispose()
        {
            pool.FreePage(browser.Uri, browser);
        }
    }
}
