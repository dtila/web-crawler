using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawlerService.Database;
using System.Net;
using System.Data.Linq;
using Contracts.DTA;
using ProxyProviderLibrary;
using WebCrawler;
using MovieCrawler.Core;
using MovieCrawler.Domain.Data;
using ProxyProviderLibrary.Data;
using MovieCrawler.Domain;

namespace WebCrawlerService
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple)]
    public partial class WebCrawlerService : IDisposable
    {
        private ProxyServerProvider _proxyProvider;
        private ICollection<MovieProviderInfo> _runningMoviesProviders;

        static WebCrawlerService()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.GetAppDataDirectory());
        }

        public WebCrawlerService()
        { 
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var proxyStorage = new ProxyFileStorage(Path.Combine(path, "proxy_list.bin"), Path.Combine(path, "proxy_states.bin"));
            _proxyProvider = new ProxyServerProvider(proxyStorage, TimeSpan.FromMinutes(1));

            using (var context = new MoviesContext())
            {
                var providers = from provider in DependencyResolver.ResolveAll<IMovieProvider>().Select(li => new { Type = li.GetType(), Instance = li })
                                join dbProvider in context.MovieProviders on provider.Instance.Name equals dbProvider.Name
                                let repeatHours = dbProvider.RefreshHours
                                select new MovieProviderInfo
                                {
                                    ProviderId = dbProvider.Id,
                                    Type = provider.Type,
                                    LastRunDate = dbProvider.LastRunDate ?? DateTime.Now.AddHours(-repeatHours),
                                    RepeatInterval = TimeSpan.FromHours(repeatHours),
                                };

                _runningMoviesProviders = new List<MovieProviderInfo>();
                foreach(var provider in providers)
                {
                    provider.Timer = new Timer(MovieProviderTimer, provider, provider.DueTime, provider.RepeatInterval);
                    _runningMoviesProviders.Add(provider);
                }
            }
        }

        [WebGet(UriTemplate = "/GetMovies")]
        Task<ICollection<Movie>> GetMovies()
        {
            using (var context = new MoviesContext())
            {
                var dlo = new DataLoadOptions();
                dlo.LoadWith<Database.Movies>(li => li.MovieStreamSet);
                dlo.LoadWith<Database.MovieStreamSet>(li => li.VideoStream);
                context.LoadOptions = dlo;

                var query = from dbMovie in context.Movies
                            select dbMovie.ToDTA();

                return Task.FromResult<ICollection<Movie>>(query.ToList());
            }
        }

        [WebGet(UriTemplate = "/GetMovieInfo?id={id}")]
        async Task<Movie> GetMovieInfo(int id)
        {
            using (var context = new MoviesContext())
            {
                var dlo = new DataLoadOptions();
                dlo.LoadWith<Database.Movies>(li => li.MovieStreamSet);
                dlo.LoadWith<Database.MovieStreamSet>(li => li.StreamCookie);
                context.LoadOptions = dlo;

                var movie = context.Movies.Single(li => li.Id == id);
                var movieStreams = new List<Contracts.DTA.MovieStreamSet>();
                var firstMovieStreamSet = movie.MovieStreamSet.FirstOrDefault();
                if (firstMovieStreamSet == null)
                    throw new InvalidOperationException(string.Format("Movie {0} does not have a stream attached", movie.Id));

                if (firstMovieStreamSet.StreamCookie.Any(li => li.ExpirationDate < DateTime.UtcNow))
                {
                    await IssueStreamParse(firstMovieStreamSet);
                    context.SubmitChanges();
                }

                for (var i = 1; i < movie.MovieStreamSet.Count; i++)
                {
                    var streamSet = movie.MovieStreamSet[i];
                    if (streamSet.StreamCookie.Any(li => li.ExpirationDate >= DateTime.UtcNow))
                    {
                        // add a record just with an id and null streams
                        movieStreams.Add(new Contracts.DTA.MovieStreamSet { Id = streamSet.Id });
                        continue;
                    }

                    movieStreams.Add(streamSet.ToDTA());
                }

                return movie.ToDTA();
            }
        }

        [WebGet(UriTemplate = "/GetStreamInfo?id={id}")]
        async Task<Contracts.DTA.MovieStreamSet> GetStreamInfo(int id)
        {
            using (var context = new MoviesContext())
            {
                var dlo = new DataLoadOptions();
                dlo.LoadWith<Database.MovieStreamSet>(li => li.StreamCookie);
                
                context.LoadOptions = dlo;
                var streamset = context.MovieStreamSet.Single(li => li.Id == id);
                if (streamset.StreamCookie.Any(li => li.ExpirationDate < DateTime.Now))
                    await IssueStreamParse(streamset);

                return streamset.ToDTA();
            }
        }

        public void Dispose()
        {
            using (_proxyProvider)
                _proxyProvider.Save();
        }

        private async Task IssueStreamParse(Database.MovieStreamSet streamSet)
        {
            /*var uri = new Uri(streamSet.AddressSource);
            var streamHost = CrawlingFactory.CreateStreamCrawler(uri);
            SetupCrawler(streamHost);
            var newMovieStreamSet = await streamHost.GetStreamSetInfoAsync(uri);

            streamSet.StreamCookie.SetSource(newMovieStreamSet.CookieCollection.Cast<Cookie>().Select(li => li.ToDBO(streamSet)));
            streamSet.StreamHeaders.SetSource(newMovieStreamSet.HeaderCollection.Select(li => li.ToDBO(streamSet)));*/
        }

        private async void MovieProviderTimer(object state)
        {
            var providerInfo = state as MovieProviderInfo;
            var log = NLog.LogManager.GetLogger("CrawlerLogger", providerInfo.Type);

            providerInfo.Timer.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                using (var providerService = new MovieProviderService(providerInfo.ProviderId))
                {
                    await providerService.ParseNewMovies();
                }
            }
            catch (InvalidDOMStructureException ex)
            {
                log.Fatal("The page structure has been changed", ex);
            }
            catch (NotImplementedException ex)
            {
                log.Fatal("Feature not implemented yet", ex);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error", ex);
            }
            finally
            {
                providerInfo.Timer.Change(providerInfo.DueTime, providerInfo.RepeatInterval);
                log.Debug("Finish");
            }
        }

        private void Builder_OnMovieLoaded(object sender, MovieBuildedEventArgs e)
        {
            throw new NotImplementedException();
        }

        class MovieProviderInfo
        {
            public int ProviderId { get; set; }
            public bool Enabled { get; set; }
            public DateTime LastRunDate { get; set; }
            public TimeSpan RepeatInterval { get; set; }

            public TimeSpan DueTime
            {
                get 
                {
                    var timespan = LastRunDate.Add(RepeatInterval) - DateTime.Now;
                    if (timespan.TotalMilliseconds < 0)
                        return TimeSpan.Zero;
                    return timespan;
                }
            }

            public Type Type { get; set; }
            public Timer Timer { get; set; }


            public void Start()
            {
                if (Timer != null)
                    return;
            }

            public void Stop()
            {
                if (Timer == null)
                    return;
                //Timer.Dispose(
            }
        }
    }
}
