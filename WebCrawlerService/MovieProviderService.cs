using MovieCrawler.Core;
using MovieCrawler.Domain.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Logging;
using WebCrawlerService.Database;

namespace WebCrawlerService
{
    class MovieProviderService : IDisposable
    {
        private MoviesContext context;
        private ILogger logger;
        private MovieProviders provider;

        public MovieProviderService(int providerId)
        {
            context = new MoviesContext();
            this.provider = context.MovieProviders.SingleOrDefault(li => li.Id == providerId);
            logger = LoggerFactory.Create("CrawlerService");
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public async Task ParseNewMovies()
        {
            var movieProvider = DependencyResolver.Resolve<IMovieProvider>(provider.Name);

            int startPageNumber = Math.Max((byte)1, provider.LastPageProcessed);
            logger.Debug(string.Format("Start the parsing process from the page {0}", startPageNumber));

            var pageSet = movieProvider.EnumerateFromPage(startPageNumber);
            foreach (var moviesPageTask in pageSet.Take(2))
            {
                var page = await moviesPageTask;
                var newMovies = (from movieSummary in page
                                 where !context.Movies.Select(li => li.Title).Contains(movieSummary.Title)
                                 select movieSummary).ToList();

                logger.Debug(string.Format("Found {0} movies and {1} are new", page.Count, newMovies.Count));

                foreach (var movie in newMovies)
                {
                    var builder = new MovieBuilder(null, movie);
                    builder.OnMovieLoaded += Builder_OnMovieLoaded;
                    await movieProvider.AddToBuilder(builder, movie);
                }

                if (++provider.LastPageProcessed >= pageSet.TotalPages)
                    provider.LastPageProcessed = 0; // All pages processed

                context.SubmitChanges();

                if (provider.LastPageProcessed == 0 && newMovies.Count < page.Count)
                    break;
            }
        }

        private void Builder_OnMovieLoaded(object sender, MovieBuildedEventArgs e)
        {
            var movieInfo = e.MovieInfo;
            var dbMovie = new Database.Movies
            {
                ProviderId = provider.Id,
                Title = movieInfo.Title,
                Year = movieInfo.Year,
                Description = movieInfo.Description,
                Link = movieInfo.Link.ToString(),
                Genres = (int)movieInfo.Genre,
                CreatedDate = DateTime.Now
            };

            if (movieInfo.CoverImage != null)
                dbMovie.Cover = movieInfo.CoverImage.ToString();

            context.SaveMovieInformation(dbMovie, movieInfo);
            provider.LastMovieTitleProcessed = movieInfo.Title;
            context.SubmitChanges();
        }
    }
}
