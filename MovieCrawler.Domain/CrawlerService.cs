using MovieCrawler.Core;
using MovieCrawler.Domain.Builder;
using MovieCrawler.Domain.Model;
using MovieCrawler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Logging;

namespace MovieCrawler.Domain
{
    public class CrawlerService
    {
        private ILogger logger;
        private IMoviesRepository repository;
        private static MovieBuilderPool pool = new MovieBuilderPool();

        public CrawlerService(IMoviesRepository repository)
        {
            this.repository = repository;
            this.logger = LoggerFactory.Create("CrawlerService");
        }

        public async Task ParseProviderNewMovies(MovieProvider provider)
        {
            var movieProvider = DependencyResolver.Resolve<IMovieProvider>(provider.Name);

            int startPageNumber = Math.Max((byte)1, provider.LastPageProcessed);
            logger.Debug(string.Format("Start the parsing process from the page {0}", startPageNumber));

            var pageSet = movieProvider.EnumerateFromPage(startPageNumber);
            foreach (var moviesPageTask in pageSet.Take(2))
            {
                var page = await moviesPageTask;
                var newMovies = repository.GetNewMovies(page);

                logger.Debug(string.Format("Found {0} movies and {1} are new", page.Count, newMovies.Count));

                foreach (var movie in newMovies)
                {
                    var builder = new MovieBuilder(pool, movie);
                    builder.OnMovieLoaded += Builder_OnMovieLoaded;
                    await movieProvider.AddToBuilder(builder, movie);
                }
            }
        }

        private void Builder_OnMovieLoaded(object sender, MovieBuildedEventArgs e)
        {
            repository.InsertMovie(e.MovieInfo);
            //provider.LastMovieTitleProcessed = movieInfo.Title;
        }
    }
}
