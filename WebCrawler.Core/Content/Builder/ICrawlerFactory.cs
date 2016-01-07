using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Content.Builder
{
    public interface ICrawlerFactory
    {
        IContentCrawler Create(Uri uri);

        bool TryCreate(Uri uri, out IContentCrawler crawler);
    }
}
