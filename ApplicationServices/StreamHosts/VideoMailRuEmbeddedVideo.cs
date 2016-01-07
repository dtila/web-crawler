using MovieCrawler.Domain;
using MovieCrawler.Domain.Builder;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCrawler.Content.Builder;

namespace MovieCrawler.ApplicationServices.StreamHosts
{
    public class VideoMailRuEmbeddedVideo : IMovieStreamHost
    {
        private static readonly Regex VideoRegex = new Regex(@"videoSrc\s?=\s?\""([^\""]+)\""", RegexOptions.Compiled);
        private Uri uri;

        public VideoMailRuEmbeddedVideo(Uri uri)
        {
            this.uri = uri;
        }

        public async Task<MovieStream> GetStreamSetAsync()
        {
            var request = WebHttp.CreateRequest(uri);
            request.CookieContainer = new System.Net.CookieContainer();
            var response = await WebHttp.GetWebResponse(request);

            var streamInfo = new MovieStream();
            streamInfo.Cookies = request.CookieContainer.GetCookies(uri);

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var match = VideoRegex.Match(sr.ReadToEnd());
                if (!match.Success)
                    throw new InvalidDOMStructureException("Unable to find the video source uri");
                streamInfo.VideoStreams.Add(new VideoStream { AVStream = match.Groups[1].Value });
            }

            return streamInfo;
        }
        public void AppendTo(IMovieBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void AppendTo(IContentBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
