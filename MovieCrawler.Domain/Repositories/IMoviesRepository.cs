using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Repositories
{
    public interface IMoviesRepository
    {
        IEnumerable<BasicMovieInfo> NewMovies(IEnumerable<BasicMovieInfo> movies);
    }
}
