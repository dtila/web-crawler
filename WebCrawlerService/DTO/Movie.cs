using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTA
{
    public class Movie
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }

        public IEnumerable<MovieGenre> Genres { get; set; }
        public IEnumerable<MovieStreamSet> Streams { get; set; }
    }

    public class MovieGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
