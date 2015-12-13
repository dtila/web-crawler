using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Infrastructure;

namespace MovieHtmlParser.Infrastructure
{
    class HttpFactory : IHttpFactory
    {
        public HttpWebRequest Create(Uri uri)
        {
            var request = WebRequest.CreateHttp(uri);
            return request;
        }

        public async Task<HttpWebResponse> GetValidResponse(HttpWebRequest request)
        {
            return (await request.GetResponseAsync()) as HttpWebResponse;
        }
    }
}
