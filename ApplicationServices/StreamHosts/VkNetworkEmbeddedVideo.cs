using MovieCrawler.ApplicationServices.Contracts;
using MovieCrawler.Domain;
using MovieCrawler.Domain.Data;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebCrawler.Core;

namespace MovieCrawler.ApplicationServices.StreamHosts
{
    class VkNetworkEmbeddedVideo : IMovieStreamHost
    {
        private static readonly Dictionary<string, VideoResolution> SupportedVideos = new Dictionary<string, VideoResolution>
        {
            { "url240", VideoResolution._240 },
            { "url360" , VideoResolution._360 },
            { "url480" , VideoResolution._480 },
            { "url720" , VideoResolution._720 },
            { "url1080" , VideoResolution._1080 },
        };
        private Uri uri;

        public VkNetworkEmbeddedVideo(Uri uri)
        {
            this.uri = uri;
        }

        public InspectMethodType GetInspectMethod(Uri uri)
        {
            return InspectMethodType.None;
        }

        public void AppendTo(MovieBuilder builder, BrowserPageInspectSubscription subscription)
        {
            throw new NotImplementedException();
        }

        public async Task<MovieStream> GetStreamSetAsync()
        {
            if (!uri.LocalPath.Equals("/video_ext.php", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidParseElementException("Not a VkNetwork embedded video!");

            var html = await WebHttp.GetHtmlDocument(uri);

            var flashEmbedded = html.GetElementbyId("flash_video_obj");
            if (flashEmbedded == null)
                flashEmbedded = html.DocumentNode.SelectSingleNode("//embed[@type='application/x-shockwave-flash']")
                                                 .ThrowExceptionIfNotExists("Not found the flash embedded element!");

            var query = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(flashEmbedded.GetAttributeValue("flashvars", null)));
            
            var streamSet = new MovieStream();
            foreach (var kvp in SupportedVideos)
            {
                string movieUrl = HttpUtility.UrlDecode(query.Get(kvp.Key));
                if (!string.IsNullOrEmpty(movieUrl))
                    streamSet.VideoStreams.Add(new VideoStream { AVStream = movieUrl, Resolution = kvp.Value });
            }

            if (streamSet.VideoStreams.Count == 0)
                throw new InvalidParseElementException("Unable to determine any valid video stream");
            return streamSet;
        }
    }
}
