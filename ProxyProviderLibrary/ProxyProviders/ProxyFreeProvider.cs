using ProxyProviderLibrary.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ProxyProviderLibrary.ProxyProviders
{
    class ProxyFreeProvider : BaseObserver<string>, IProxySource
    {
        private static readonly Regex MatchRegex = new Regex(@"href=""([^\""]+?output=txt)\""", RegexOptions.Compiled);
        private static readonly Regex IpRegex = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}", RegexOptions.Compiled);

        public string Name
        {
            get { return "ProxyFreeProvider"; }
        }

        public TimeSpan RefreshTime
        {
            get { return TimeSpan.FromDays(2); }
        }

        public async Task Start(CancellationToken token)
        {
            foreach (var task in EnumeratePages())
            {
                var link = await task;

                try
                {
                    var pageCount = 0;
                    var request = CreateRequest(link);
                    var response = await request.GetResponseAsync();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        await reader.ReadLineAsync(); // skip header
                        while (!reader.EndOfStream && !token.IsCancellationRequested)
                        {
                            var line = await reader.ReadLineAsync();
                            if (!IpRegex.IsMatch(line))
                                continue;

                            RaiseOnNext(line);
                            pageCount++;
                        }

                        RaiseOnCompleted();
                    }

                    if (pageCount > 0)
                        break;
                }
                catch
                {
                }
            }
        }

        private HttpWebRequest CreateRequest(Uri address)
        {
            var request = HttpWebRequest.Create(address) as HttpWebRequest;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:28.0) Gecko/20100101 Firefox/28.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.CookieContainer = new CookieContainer();
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            return request;
        }

        private IEnumerable<Task<Uri>> EnumeratePages()
        {
            var feedFormatter = new System.ServiceModel.Syndication.Atom10FeedFormatter();
            using (XmlReader reader = XmlReader.Create("https://sites.google.com/site/proxyfree4u/proxy-list/posts.xml"))
                feedFormatter.ReadFrom(reader);

            foreach (var feeds in feedFormatter.Feed.Items.OrderByDescending(li => li.PublishDate))
            {
                var pageAddress = feeds.Links.FirstOrDefault(li => li.MediaType == "text/html");
                if (pageAddress == null)
                    yield break;

                yield return GetDownloadLink(pageAddress.Uri);
            }
        }

        private async Task<Uri> GetDownloadLink(Uri pageAddress)
        {
            var request = CreateRequest(pageAddress);
            var response = await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();
                var match = MatchRegex.Match(content);
                if (match.Success)
                    return new Uri(HttpUtility.HtmlDecode(match.Groups[1].Value));
            }
            return null;
        }
    }
}
