using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Infrastructure;

namespace MovieCrawler.Domain
{
    class WebHttp
    {
        private static IHttpFactory Factory = DependencyResolver.Resolve<IHttpFactory>();

        public static Task<HttpWebResponse> GetResponse(Uri uri, string requestMethod)
        {
            var request = Factory.Create(uri);
            request.Method = "HEAD";
            return Factory.GetValidResponse(request);
        }

        public static bool IsMovieContent(HttpWebResponse response)
        {
            return string.Equals(response.ContentType, "application/octet-stream", StringComparison.InvariantCultureIgnoreCase) || response.ContentLength > 1000000 /*1 mil*/;
        }
    }
}
