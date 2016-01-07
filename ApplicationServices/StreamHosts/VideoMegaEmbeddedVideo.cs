using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using MovieCrawler.Domain.Model;
using MovieCrawler.Domain;
using WebCrawler.Logging;
using WebCrawler;
using MovieCrawler.Domain.Builder;
using WebCrawler.Content.Builder;

namespace MovieCrawler.ApplicationServices.StreamHosts
{
    public class VideoMegaEmbeddedVideo : IMovieStreamHost
    {
        // Valid Json 
        //{"logo.file":"http://videomega.tv/imgs/logo_player.png","logo.link":"http://videomega.tv",allowscriptaccess:"always",image:"http://st59.u1.videomega.tv/videos/screenshots/221a98c5be694cade314fe8ca24d235f.jpg",file: "http://st59.u1.videomega.tv/v/221a98c5be694cade314fe8ca24d235f.mp4?st=JCRctAiuvnlbZXLzLmA7OA",flashplayer: "http://videomega.tv/player.swf","provider":"http","http.startparam":"start", height: "100%", width: "100%",wmode:"transparent",frontcolor:"#ffffff",backcolor:"#000", stretching:"uniform",controlbar: "bottom", dock: "false", allowfullscreen: "true","plugins": {"like-1":{},"viral-2": {"functions":"embed","onpause": "false","callout":"none","oncomplete":"false","embed":unescape("%3c%69%66%72%61%6d%65%20%77%69%64%74%68%3d%22%38%37%30%22%20%68%65%69%67%68%74%3d%22%34%33%30%22%20%73%63%72%6f%6c%6c%69%6e%67%3d%22%6e%6f%22%20%66%72%61%6d%65%62%6f%72%64%65%72%3d%22%30%22%20%73%72%63%3d%22%68%74%74%70%3a%2f%2f%76%69%64%65%6f%6d%65%67%61%2e%74%76%2f%69%66%72%61%6d%65%2e%70%68%70%3f%72%65%66%3d%65%4f%66%57%62%47%42%41%57%51%26%77%69%64%74%68%3d%38%37%30%26%68%65%69%67%68%74%3d%34%33%30%22%3e%3c%2f%69%66%72%61%6d%65%3e")}}}
        //playlist: [{
        //        file: "/assets/sintel.mp4",
        //        image: "/assets/sintel.jpg",
        //        tracks: [{ 
        //            file: "/assets/captions-en.vtt", 
        //            label: "English",
        //            kind: "captions",
        //            "default": true 
        //        },{ 
        //            file: "/assets/captions-fr.vtt", 
        //            kind: "captions",
        //            label: "French"
        //        }]
        //    }]

        private const string Host = "videomega.tv";
        private const string SearchPattern = "document.write(unescape(\"";
        private ILogger logger;
        private Uri uri;

        public VideoMegaEmbeddedVideo(Uri uri)
        {
            logger = LoggerFactory.Create("videomega.tv");
            this.uri = uri;
        }

        public async Task<MovieStream> GetStreamSetAsync()
        {
            var response = await WebHttp.GetWebResponse(uri);
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = await reader.ReadToEndAsync();
                int startIndex = content.IndexOf(SearchPattern, StringComparison.CurrentCultureIgnoreCase);
                if (startIndex < 0)
                    throw new InvalidDOMStructureException("Unable to find the start point of the escaped flash");

                startIndex += SearchPattern.Length;

                var endIndex = content.IndexOf('\"', startIndex);
                if (endIndex < 0)
                    throw new InvalidDOMStructureException("Unable to find the end point of the escaped flash");
                
                var flashContent = Utilities.EscapeJavaScript(content.Substring(startIndex, endIndex - startIndex));

                startIndex = flashContent.IndexOf('{');
                if (startIndex < 0)
                    throw new InvalidDOMStructureException("Unable to find the start character for the Json data");
                endIndex = flashContent.LastIndexOf('}');
                if (endIndex < 0)
                    throw new InvalidDOMStructureException("Unable to find the end character for the Json data");

                var json = Newtonsoft.Json.Linq.JObject.Parse(flashContent.Substring(startIndex, endIndex - startIndex + 1));
                var movieStreamSet = new MovieStream();

                JToken token;
                if (json.TryGetValue("provider", out token) && !token.ToString().Equals("http", StringComparison.CurrentCultureIgnoreCase))
                    logger.Warning(string.Format("An non HTTP provider was found: {0}", token.ToString()));

                if (!json.TryGetValue(((string)token) + ".startparam", out token))
                    logger.Warning("Unable to find a valid value for the http.startparam");

                if (!json.TryGetValue("file", out token))
                    throw new InvalidDOMStructureException("Unable to find the 'file' value in the JSON data");
                
                movieStreamSet.VideoStreams.Add(new VideoStream
                {
                    AVStream = CreateUri((string)token).ToString(),
                });

                if (json.TryGetValue("tracks", out token))
                {
                    foreach(var jcaption in token.OfType<JObject>())
                    {
                        var caption = new Caption
                        {
                            Language = (string)jcaption["label"],
                            Address = CreateUri((string)jcaption["file"]).ToString()
                        };
                        movieStreamSet.Captions.Add(caption);
                    }
                }

                return movieStreamSet;
            }
        }

        private Uri CreateUri(string path)
        {
            UriBuilder builder = null;
            try
            {
                builder = new UriBuilder(path);
            }
            catch (Exception)
            {
                builder = new UriBuilder("http", Host, 80, path);
            }

            return builder.Uri;
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
