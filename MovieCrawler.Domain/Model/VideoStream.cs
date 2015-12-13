using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Model
{
    public class VideoStream
    {
        public string AVStream { get; set; }
        public VideoResolution Resolution { get; set; }
        public VideoType Type { get; set; }
    }

    public class Caption
    {
        public string Language { get; set; }
        public string Address { get; set; }
    }
}
