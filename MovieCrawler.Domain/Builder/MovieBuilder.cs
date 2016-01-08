using MovieCrawler.Core;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Infrastructure;
using WebCrawler.Logging;
using System.Net;
using WebCrawler.Data;
using WebCrawler.Content.Builder;
using WebCrawler.Content;
using System.Threading;

namespace MovieCrawler.Domain.Builder
{
    public class MovieBuilder : IMovieBuilder
    {
        private MovieInfo movieInfo;
        private MovieBuilderPool pool;
        private List<Uri> videoStreams;
        private ILogger logger;
        private ICrawlerFactory crawlerFactory;
        private int waitingEvents;

        public event EventHandler<MovieBuildedEventArgs> OnMovieLoaded;

        public MovieInfo MovieInfo { get { return movieInfo; } }

        public MovieBuilder(MovieBuilderPool pool, BasicMovieInfo basicMovieInfo)
        {
            this.pool = pool;
            this.videoStreams = new List<Uri>();
            this.movieInfo = new MovieInfo(basicMovieInfo);
            this.waitingEvents = 0;

            crawlerFactory = DependencyResolver.Resolve<ICrawlerFactory>();
            logger = LoggerFactory.Create("MovieBuilder (" + basicMovieInfo.Title + ")");
        }

        public void Enqueue(IBrowserContent content)
        {
            pool.Enqueue(content, this);
        }

        public async Task Enqueue(ILinkUnscrambler scrambler)
        {
            var uri = await scrambler.GetLinkAsync();
            Enqueue(uri);
        }

        public IMovieBuilderAsyncOperation BeginOperation()
        {
            return new PendingBuildAsyncOperation(this);
        }

        public void Enqueue(Uri uri)
        {
            IContentCrawler crawler = crawlerFactory.Create(uri);
            var movieCrawler = crawler as IMovieCrawler;
            if (movieCrawler != null)
                movieCrawler.AppendTo(this);
            else
                crawler.AppendTo(this);
        }

        public void Enqueue(IMovieProvider sender, Uri uri)
        {
            Enqueue(uri);
            EnqueueCompleted();
        }

        private void EnqueueCompleted()
        {
            throw new NotImplementedException();
        }

        private void RaiseMovieLoaded()
        {
            try
            {
                if (string.IsNullOrEmpty(movieInfo.Description))
                    throw new InvalidOperationException("The movie does not has a description");

                if (movieInfo.Streams.Count == 0)
                    throw new InvalidOperationException("Unable to complete a movie without any available stream");
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Unable to raise a movie loaded event to an invalid movie", ex);
            }

            var h = OnMovieLoaded;
            if (h != null)
                h(this, new MovieBuildedEventArgs(movieInfo));
        }


        private class PendingBuildAsyncOperation : IMovieBuilderAsyncOperation
        {
            private MovieBuilder builder;

            public PendingBuildAsyncOperation(MovieBuilder builder)
            {
                this.builder = builder;
                Interlocked.Increment(ref builder.waitingEvents);
            }

            public IMovieBuilder Builder { get { return builder; } }

            public void Dispose()
            {
                if (Interlocked.Decrement(ref builder.waitingEvents) == 0)
                    builder.RaiseMovieLoaded();
            }
        }

    }

    public class MovieBuildedEventArgs : EventArgs
    {
        public MovieInfo MovieInfo { get; private set; }

        public MovieBuildedEventArgs(MovieInfo movieInfo)
        {
            this.MovieInfo = movieInfo;
        }
    }
}
