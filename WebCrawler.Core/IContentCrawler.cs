using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Core
{
    public interface IContentCrawler
    {
        InspectMethodType GetInspectMethod(Uri uri);
    }

    [Flags]
    public enum InspectMethodType
    {
        None = 0,
        Raw = 1,
        Browser = 2
    }
}
