using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTA
{
    public class MovieStreamSet
    {
        public int Id { get; set; }
        public ICollection<MovieStream> VideoStreams { get; set; }
        public ICollection<MovieCaption> Captions { get; set; }

        public MovieStreamSet()
        {
            VideoStreams = new List<MovieStream>();
            Captions = new List<MovieCaption>();
        }
    }
}
