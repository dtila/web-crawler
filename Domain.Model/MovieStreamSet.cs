using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.ApplicationServices.Domain.Model
{
    public class MovieStreamSet
    {
        public IList<VideoStream> VideoStreams { get; set; }
        public IList<Caption> Captions { get; set; }

        public CookieCollection Cookies { get; set; }
        public NameValueCollection Headers { get; set; }

        public MovieStreamSet()
        {
            Cookies = new System.Net.CookieCollection();
            Headers = new NameValueCollection();
            VideoStreams = new List<VideoStream>();
            Captions = new List<Caption>();
        }
    }
}
