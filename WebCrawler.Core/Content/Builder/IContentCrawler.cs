using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data;

namespace WebCrawler.Content.Builder
{
    public interface IContentCrawler
    {
        void AppendTo(IContentBuilder builder);
    }
}
