using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Model
{
    public class MovieStream
    {
        public IList<VideoStream> VideoStreams { get; set; }
        public IList<Caption> Captions { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public CookieCollection Cookies { get; set; }
        public IDictionary<string, string> Headers { get; set; }

        public MovieStream()
        {
            Cookies = new System.Net.CookieCollection();
            Headers = new Dictionary<string, string>();
            VideoStreams = new List<VideoStream>();
            Captions = new List<Caption>();
        }
    }
}
