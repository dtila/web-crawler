using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Infrastructure;
using WebCrawler.Logging;

namespace MovieCrawler.ApplicationServices
{
    public static class WebHttp
    {
        private static ILogger logger;

        public const int ProxyNumberRetries = 20;

        public static IHttpResponseMonitor ResponseMonitor;
        private static IHttpFactory httpFactory;

        static WebHttp()
        {
            logger = LoggerFactory.Create("WebHttpRequests");
            httpFactory = DependencyResolver.Resolve<IHttpFactory>();
        }

        public static HttpWebRequest CreateRequest(Uri uri)
        {
            return httpFactory.Create(uri);
        }

        public static Task<HttpWebResponse> GetWebResponse(HttpWebRequest request)
        {
            return httpFactory.GetValidResponse(request);
        }

        public static Task<HttpWebResponse> GetWebResponse(Uri uri)
        {
            return GetWebResponse(CreateRequest(uri));
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
                catch (WebException)
                {
                    if (ResponseMonitor != null)
                        ResponseMonitor.Fail(request.Address);
                    //Logger.Warn(string.Format("Unable to create the request because of a proxy failure connect. Retry number: {0}", i), ex);
                    continue;
                }
            }

            throw new InvalidOperationException("Unable to perform a request because the maximum retry count has been reached");
        }
    }
}
