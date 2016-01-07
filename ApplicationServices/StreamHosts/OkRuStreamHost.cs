using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieCrawler.Domain.Model;
using WebCrawler.Content.Builder;
using MovieCrawler.Domain.Builder;
using MovieCrawler.Domain;

namespace MovieCrawler.ApplicationServices.StreamHosts
{
    class OkRuStreamHost : IMovieStreamHost
    {
        private Uri uri;

        public OkRuStreamHost(Uri uri)
        {
            this.uri = uri;
        }

        public void AppendTo(IContentBuilder builder)
        {
            var movieBuilder = builder as MovieBuilder;
            // TODO: here we have the URI and the builder and we should add a stream
            //movieBuilder.Enqueue(this, uri);
        }

        public Task<MovieStream> GetStreamSetAsync()
        {
            throw new NotImplementedException();
        }

        public class VideoPlayerMetadataResponse
        {
            public string provider { get; set; }
            public string service { get; set; }
            public bool owner { get; set; }
            public bool voted { get; set; }
            public int likeCount { get; set; }
            public bool subscribed { get; set; }
            public int slot { get; set; }
            public int siteZone { get; set; }
            public bool showAd { get; set; }
            public int fromTime { get; set; }
            public Author author { get; set; }
            public Movie movie { get; set; }
            public AdmanMetadata admanMetadata { get; set; }
            public int partnerId { get; set; }
            public List<Video> videos { get; set; }
            public string metadataEmbedded { get; set; }
            public string metadataUrl { get; set; }
            public List<string> failoverHosts { get; set; }
            public string maxAllowedQuality { get; set; }
            public Autoplay autoplay { get; set; }
            public Security security { get; set; }
        }

        public class Author
        {
            public string id { get; set; }
            public string name { get; set; }
            public string profile { get; set; }
        }

        public class CollageInfo
        {
            public string url { get; set; }
            public int frequency { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public int count { get; set; }
        }

        public class Movie
        {
            public string id { get; set; }
            public string movieId { get; set; }
            public string contentId { get; set; }
            public string poster { get; set; }
            public string duration { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string link { get; set; }
            public CollageInfo collageInfo { get; set; }
            public string status { get; set; }
        }

        public class AdmanMetadata
        {
        }

        public class Video
        {
            public string name { get; set; }
            public string url { get; set; }
            public int seekSchema { get; set; }
            public bool disallowed { get; set; }
        }

        public class Autoplay
        {
            public bool autoplayEnabled { get; set; }
            public bool timeFromEnabled { get; set; }
            public bool noRec { get; set; }
            public bool fullScreenExit { get; set; }
            public string vitrinaSection { get; set; }
        }

        public class Security
        {
            public string url { get; set; }
            public string cookie { get; set; }
        }
    }
}
