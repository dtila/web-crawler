using MovieCrawler.Domain;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.Contracts
{
    public interface IMovieStreamHost : IMovieCrawler
    {
        Task<MovieStream> GetStreamSetAsync();
    }
}
