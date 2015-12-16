using MovieCrawler.ApplicationServices.Contracts;
using MovieCrawler.Core;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Core;
using WebCrawler.Infrastructure;
using WebCrawler.Logging;
using System.Net;

namespace MovieCrawler.Domain.Data
{
    public class MovieBuilder
    {
        private MovieInfo movieInfo;
        private MovieBuilderPool pool;
        private List<Uri> videoStreams;
        private ILogger logger;

        public event EventHandler<MovieBuildedEventArgs> OnMovieLoaded;

        public MovieInfo MovieInfo { get { return movieInfo; } }

        public MovieBuilder(MovieBuilderPool pool, BasicMovieInfo basicMovieInfo)
        {
            logger = LoggerFactory.Create("MovieBuilder (" + basicMovieInfo.Title + ")");
            this.pool = pool;
            this.videoStreams = new List<Uri>();
            this.movieInfo = new MovieInfo(basicMovieInfo);
        }

        public void AddStream(MovieStream stream)
        {
            movieInfo.Streams.Add(stream);
        }

        public void Enqueue(IMovieProvider sender, Uri movieSetUri)
        {
            // TODO: Enqueue only the browser requests
            var factory = DependencyResolver.Resolve<IHttpFactory>();
            var request = factory.Create(movieSetUri);
            request.Method = "HEAD";
            factory.GetValidResponse(request).ContinueWith(task => 
            {
                if (task.IsFaulted)
                {
                    logger.Error("The URL '{0}' has a faulted request, so it's posponed later");
                    lock(this)
                    { 
                        videoStreams.Add(movieSetUri);
                    }
                    pool.Enqueue(movieSetUri, this);
                    return;
                }

                CheckWebResponse(movieSetUri, task.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Enqueue(IMovieStreamHost sender, Uri uri)
        {
            var result = sender.GetInspectMethod(uri);
            //if (result == InspectMethodType.)
        }

        private bool RawContent(IMovieCrawler sender, Uri uri, out InspectMethodType inspectType)
        {
            inspectType = sender.GetInspectMethod(uri);
            if (inspectType == InspectMethodType.None)
                throw new InvalidOperationException(string.Format("The crawler '{0}' can not handle the '{1}' address", sender, uri));

            if ((inspectType & InspectMethodType.Browser) != 0)
            {
                pool.Enqueue(uri, this);
                return false;
            }

            return true;
        }

        private void CheckWebResponse(Uri originalUri, HttpWebResponse result)
        {
            //throw new NotImplementedException();
        }

        internal void OnPageLoaded(BrowserPageInspectSubscription pageSubscription)
        {
            var crawler = CreateMovieCrawler(pageSubscription.CurrentUri);
            crawler.AppendTo(this, pageSubscription);

            bool movieCompleted = false;
            lock (this)
            {
                if (!videoStreams.Remove(pageSubscription.OriginalUri))
                    throw new InvalidOperationException(string.Format("The '{0}' page has been loaded into builder, but is not recognised by this", pageSubscription.OriginalUri));
                movieCompleted = videoStreams.Count == 0;
            }

            if (movieCompleted)
                RaiseMovieLoaded();
        }

        private static IMovieCrawler CreateMovieCrawler(Uri uri)
        {
            var crawler = DependencyResolver.Resolve<ICrawlerFactory>().Create(uri);
            var crawler_ = crawler as IMovieCrawler;
            if (crawler_ == null)
                throw new InvalidCastException("Unable to dispatch to '" + crawler + "' because is not a movie crawler");
            return crawler_;
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
