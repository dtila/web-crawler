using MovieCrawler.Core;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Content.Builder;

namespace MovieCrawler.Domain.Builder
{
    public interface IMovieBuilder : IContentBuilder
    {
        MovieInfo MovieInfo { get; }

        void AddStream(MovieStream stream);


        void Enqueue(IMovieProvider sender, Uri uri);
    }
}
