using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Logging;

namespace MovieCrawler.ApplicationServices
{
    public static class WebHttp
    {
        private static ILogger logger;

        public const int ProxyNumberRetries = 20;

        public static Func<IWebProxy> ProxyFactoryDelegate = () => null;
        public static IHttpResponseMonitor ResponseMonitor;

        static WebHttp()
        {
            logger = LoggerFactory.Create("WebHttpRequests");
        }

        private static readonly IList<string> UserAgents = new[]
        {
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:28.0) Gecko/20100101 Firefox/28.0",
            "Mozilla/5.0 (Linux; U; Android 4.2.2; en-us; B92M Build/JZO54K) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
            "Mozilla/5.0 (Linux; U; Android 4.0.3; fr-fr; HTC/Sensation/3.32.163.12 Build/IML74K) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/",
            "Mozilla/5.0 (Linux; Android 4.4.2; SM-N900S Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Version/1.6 Chrome/34.0.1847.76 Mobile Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.71 Safari/537.36",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727)",
            "Mozilla/4.0 (compatible; MSIE 6.1; Windows NT)",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:9.0) Gecko/20100101 Firefox/9.0",
            "Mozilla/5.0 (X11; Linux ppc; rv:5.0) Gecko/20100101 Firefox/5.0",
            "Mozilla/5.0 (X11; Linux AMD64) Gecko Firefox/5.0",
            "Mozilla/5.0 (X11; FreeBSD amd64; rv:5.0) Gecko/20100101 Firefox/5.0",
            "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0",
            "Mozilla/5.0 (compatible; Konqueror/3.1-rc2; i686 Linux; 20021221)",
            "Mozilla/5.0 (compatible; Konqueror/2.1-rc2; i686 Linux; 20000128)",
            "Mozilla/5.0 (compatible; Konqueror/1.1; i686 Linux; 10028741)",
            "Opera/7.11 (Windows NT 5.1; U) [pl]",
            "Opera/7.11 (Windows NT 5.1; U) [en]",
            "Opera/7.11 (Windows NT 5.0; U) [de]"
        };

        public static HttpWebRequest CreateRequest(Uri uri)
        {
            var rand = new Random().Next(UserAgents.Count);
            var httpRequest = WebRequest.CreateHttp(uri);
            httpRequest.UserAgent = UserAgents[rand];
            httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            if (rand % 5 == 0)
                httpRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            else
                httpRequest.Headers.Add("Accept-Language", "en-UK,en;q=0.5");

            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (ProxyFactoryDelegate != null)
                httpRequest.Proxy = ProxyFactoryDelegate();

            return httpRequest;
        }

        public static async Task<HtmlAgilityPack.HtmlDocument> GetHtmlDocument(Uri address)
        {
            for (var i = 0; i < ProxyNumberRetries; i++)
            {
                var request = CreateRequest(address);

                try
                {
                    var response = await request.GetResponseAsync();
                    var html = new HtmlAgilityPack.HtmlDocument();
                    html.Load(response.GetResponseStream());
                    if (html.DocumentNode.OuterHtml.Length < 10)
                        throw new WebException("The root document is not a valid document. Skip this proxy.");

                    if (ResponseMonitor != null)
                    {
                        ResponseMonitor.Success(request.Address, request.Proxy.GetProxy(request.Address));
                    }

                    return html;
                }
                catch (WebException ex)
                {
                    if (ResponseMonitor != null)
                        ResponseMonitor.Fail(request.Address);
                    //Logger.Warn(string.Format("Unable to create the request because of a proxy failure connect. Retry number: {0}", i), ex);
                    continue;
                }
            }

            throw new InvalidOperationException("Unable to perform a request because the maximum retry count has been reached");
        }

        public static Task<WebResponse> GetWebResponse(Uri address)
        {
            return ProxyRetryGuardedSection(() =>
            {
                var request = CreateRequest(address);
                return request.GetResponseAsync();
            });
        }


        public static async Task<T> ProxyRetryGuardedSection<T>(Func<Task<T>> action)
        {
            for (var i = 0; i < ProxyNumberRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch (WebException ex)
                {
                    logger.Warning("Unable to create the request because of a proxy failure connect", ex);
                    continue;
                }
            }

            throw new InvalidOperationException("Unable to perform a request because the maximum retry count has been reached");
        }

    }
}
