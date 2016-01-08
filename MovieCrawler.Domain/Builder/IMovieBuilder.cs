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

        IMovieBuilderAsyncOperation BeginOperation();

        void Enqueue(IMovieProvider sender, Uri uri);
    }

    /// <summary>
    /// Keep a reference to a <see cref="IMovieBuilder"/>
    /// A reference to a builder denotes that there is a pending 
    /// </summary>
    public interface IMovieBuilderAsyncOperation : IDisposable
    {
        IMovieBuilder Builder { get; }
    }
}
