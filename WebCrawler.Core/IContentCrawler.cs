using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data;

namespace WebCrawler.Core
{
    public interface IContentCrawler
    {
        void AppendTo(IContentBuilder builder);
    }

    [Flags]
    public enum InspectMethodType
    {
        None = 0,
        Raw = 1,
        Browser = 2
    }
}
