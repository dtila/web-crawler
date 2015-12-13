using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Infrastructure
{
    public interface IHttpFactory
    {
        HttpWebRequest Create(Uri uri);

        /// <summary>
        /// Retrieve a valid HTTP Response from the specified request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<HttpWebResponse> GetValidResponse(HttpWebRequest request);
    }
}
