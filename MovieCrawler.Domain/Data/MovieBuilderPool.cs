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
using WebCrawler.Data;
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

        internal void Enqueue(IBrowserContent request, MovieBuilder builder)
        {
            IBrowser browser;
            if (runningPages.Count < MaxRunningPages && availableWorkers.TryDequeue(out browser))
            {
                try
                {
                    lock(this)
                    {
                        runningPages.Add(new RunningPage(request.Uri, builder, browser));
                    }
                    browser.Navigate(request.Uri).ContinueWith(task => PageLoaded(new BrowserPageInspectSubscription(this, request, browser, builder)));
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Unable to navigate to the page {0}", request.Uri), ex);
                    availableWorkers.Enqueue(browser);
                    throw;
                }
            }

            logger.Info("An address has been enqueued: " + request.Uri);
            enqueuedPages.Enqueue(new Page(request, builder));
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
                Enqueue(enqueuedPage.Request, enqueuedPage.Builder);
            }
        }

        struct Page
        {
            public IBrowserContent Request;
            public MovieBuilder Builder;

            public Page(IBrowserContent request, MovieBuilder builder)
            {
                Request = request;
                Builder = builder;
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
    class BrowserPageInspectSubscription : IBrowserPageInspectSubscription, IDisposable
    {
        private MovieBuilderPool pool;
        private IBrowser browser;
        private IBrowserContent request;
        private MovieBuilder builder;

        public IBrowserContent Request { get { return request; } }
        public Uri OriginalUri { get { return request.Uri; } }
        public Uri CurrentUri { get { return browser.Uri; } }
        public IBrowserPage Page { get { return browser.Page; } }
        public IContentBuilder Builder { get { return builder; } }

        internal BrowserPageInspectSubscription(MovieBuilderPool pool, IBrowserContent request, IBrowser browser, MovieBuilder builder)
        {
            this.builder = builder;
            this.request = request;
            this.browser = browser;
            this.pool = pool;
        }

        internal void Inspect()
        {
            request.Fullfill(this);
        }

        public void Dispose()
        {
            pool.FreePage(browser.Uri, browser);
        }

        public void Release()
        {
            Dispose();
        }
    }
}
