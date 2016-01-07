using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Diagnostics
{
    public interface IHttpResponseMonitor
    {
        void Success(Uri uri, Uri proxyUri);
        void Fail(Uri uri);
    }
}
