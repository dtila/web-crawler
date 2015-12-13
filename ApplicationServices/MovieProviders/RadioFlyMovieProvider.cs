using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices.MovieProviders
{
    /*class RadioFlyMovieProvider : BaseMovieProvider
    {
        private static readonly string PageAddress = "http://www.radiofly.ws/filme-online-gratis.php";

        public override string Name
        {
            get { return "RadioFly"; }
        }

        public override Task<Commons.MovieInfo> GetMovieInfoAsync(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<Task<ICollection<BasicMovieInfo>>> GetEnumerator()
        {
            return new MoviesEnumerator(this);
        }

        private async Task<ICollection<BasicMovieInfo>> ParseMovies()
        {
            return null;
        }


        class MoviesEnumerator : IEnumerator<Task<ICollection<BasicMovieInfo>>>
        {
            private RadioFlyMovieProvider _provider;
            private bool _finished;

            public MoviesEnumerator(RadioFlyMovieProvider provider)
            {
                _provider = provider;
                _finished = false;
            }

            public Task<ICollection<BasicMovieInfo>> Current { get; private set; }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_finished)
                {
                    Current = null;
                    return false;
                }

                Current = _provider.ParseMovies();
                _finished = true;
                return true;
            }

            public void Reset()
            {
                _finished = false;
            }

            public void Dispose()
            {
            }
        }

    }*/
}
