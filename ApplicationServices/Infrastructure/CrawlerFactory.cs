using MovieCrawler.ApplicationServices.LinkScramblers;
using MovieCrawler.ApplicationServices.StreamHosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Content.Builder;

namespace MovieCrawler.ApplicationServices.Infrastructure
{
    class CrawlerFactory : ICrawlerFactory
    {
        public IContentCrawler Create(Uri uri)
        {
            switch (uri.Host.ToLowerInvariant())
            {
                /// Stream hosts
                case "vk.com":
                    if (uri.LocalPath.Equals("/video_ext.php", StringComparison.InvariantCultureIgnoreCase))
                        return new VkNetworkEmbeddedVideo(uri);
                    throw new NotImplementedException(string.Format("Unable to create a crawler in the domain 'vk.com' for the path '{0}'", uri.LocalPath));
                case "api.video.mail.ru": return new VideoMailRuEmbeddedVideo(uri);
                case "videomega.tv":
                    if (uri.LocalPath.Equals("/iframe.php", StringComparison.InvariantCultureIgnoreCase))
                        return new VideoMegaEmbeddedVideo(uri);
                    throw new NotImplementedException(string.Format("Unable to create a crawler in the domain 'videomega.tv' for the path '{0}'", uri.LocalPath));
                case "ok.ru": return new OkRuStreamHost(uri);

                /// Link scrambler
                case "adf.ly": return new AdFlyLinkScrambler(uri);
            }

            throw new NotImplementedException("Unable to create a crawler for the uri " + uri.ToString());
        }

        public bool TryCreate(Uri uri, out IContentCrawler crawler)
        {
            try
            {
                crawler = Create(uri);
                return true;
            }
            catch
            {
                crawler = null;
                return false;
            }
        }
    }
}
