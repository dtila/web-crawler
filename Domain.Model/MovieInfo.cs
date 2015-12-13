using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices.Domain.Model
{
    [DebuggerDisplay("{Description}")]
    public class MovieInfo : BasicMovieInfo
    {
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public MovieGenre Genre { get; set; }

        /// <summary>
        /// The embedded streams available for the movie
        /// </summary>
        public ICollection<MovieStreamSet> Streams { get; set; }

        public MovieInfo()
        {
            Streams = new List<MovieStreamSet>();
        }
    }


    [DebuggerDisplay("{Title}")]
    public class BasicMovieInfo
    {
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Link { get; set; }
    }
}
