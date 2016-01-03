using MovieCrawler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieCrawler.Domain.Model;

namespace MovieHtmlParser.Infrastructure
{
    class MoviesRepository : IMoviesRepository
    {
        public IList<BasicMovieInfo> GetNewMovies(IEnumerable<BasicMovieInfo> movies)
        {
            return movies.ToList();
        }

        public void InsertMovie(MovieInfo movieInfo)
        {
        }
    }
}
