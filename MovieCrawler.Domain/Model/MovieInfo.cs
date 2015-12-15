using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Model
{
    [DebuggerDisplay("{Description}")]
    public class MovieInfo : BasicMovieInfo
    {
        public string Description { get; set; }
        public Uri CoverImage { get; set; }
        public MovieGenre Genre { get; private set; }

        /// <summary>
        /// The embedded streams available for the movie
        /// </summary>
        public ICollection<MovieStream> Streams { get; set; }

        public MovieInfo(BasicMovieInfo movieInfo) : base(movieInfo)
        {
            Streams = new List<MovieStream>();
        }

        public void LoadGenresFrom(string genres, char delimiter = ',')
        {
            Genre = MovieHelpers.RetrieveMovieGenres(genres.Split(delimiter).Select(li => li.Trim()));
        }

        public void LoadGenresFrom(IEnumerable<string> genres)
        {
            Genre = MovieHelpers.RetrieveMovieGenres(genres);
        }
    }


    [DebuggerDisplay("{Title}")]
    public class BasicMovieInfo
    {
        private string title;
        private int? year;

        public string Title { get { return title; } }
        public int? Year { get { return year; } }
        public Uri Link { get; private set; }

        public BasicMovieInfo(string title, int? year, Uri link)
        {
            this.title = title;
            this.year = year;
            this.Link = link;
            Verify();
        }

        public BasicMovieInfo(string title, Uri link)
        {
            if (!MovieHelpers.CanExtract(title, out this.title, out this.year))
                throw new ArgumentException("The title is not in a valid format allowed");
            this.Link = link;
            Verify();
        }

        public BasicMovieInfo(BasicMovieInfo movieInfo)
        {
            title = movieInfo.Title;
            year = movieInfo.Year;
            this.Link = movieInfo.Link;
            Verify();
        }

        private void Verify()
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Empty or null title");
            if (Link == null)
                throw new ArgumentNullException("link");
        }

    }
}
