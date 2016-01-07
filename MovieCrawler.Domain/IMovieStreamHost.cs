using MovieCrawler.Domain;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain
{
    public interface IMovieStreamHost : IMovieCrawler
    {
        Task<MovieStream> GetStreamSetAsync();
    }
}
