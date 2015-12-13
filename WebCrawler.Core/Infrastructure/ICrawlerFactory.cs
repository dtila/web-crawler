using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core;

namespace WebCrawler.Infrastructure
{
    public interface ICrawlerFactory
    {
        ICrawler Create(Uri uri);
        bool TryCreate(Uri uri, out ICrawler crawler);
    }
}
