using MovieCrawler.Domain;
using MovieCrawler.Domain.Builder;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieCrawler.Core
{
    public interface IMovieProvider : IMovieCrawler
    {
        string Name { get; }

        Task AddToBuilder(IMovieBuilder builder, BasicMovieInfo movie);

        /// <summary>
        /// Enumerate the movies from the current provider starting with the specified page. 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        IPageSet EnumerateFromPage(int page);
    }

    public interface IPageSet : IEnumerable<Task<ICollection<BasicMovieInfo>>>
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        bool HasTotalPages { get; }
    }

    public interface IPageUriCollection : IReadOnlyCollection<Uri>
    {

    }
}
