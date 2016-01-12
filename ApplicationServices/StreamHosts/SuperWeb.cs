using MovieCrawler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieCrawler.Domain.Builder;
using MovieCrawler.Domain.Model;
using WebCrawler.Content.Builder;

namespace MovieCrawler.ApplicationServices.StreamHosts
{
    class SuperWeb : IMovieStreamHost
    {
        private Uri uri;
        public SuperWeb(Uri uri)
        {
            this.uri = uri;
        }

        public void AppendTo(IContentBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void AppendTo(IMovieBuilder builder)
        {
            throw new NotImplementedException();
        }

        public Task<MovieStream> GetStreamSetAsync()
        {
            throw new NotImplementedException();
        }
    }
}
