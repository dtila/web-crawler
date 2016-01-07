using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;
using WebCrawler.Content.Builder;

namespace WebCrawler.Browser
{
    public interface IBrowserPageInspectSubscription
    {
        Uri OriginalUri { get; }
        Uri CurrentUri { get; }
        IBrowserPage Page { get; }
        IContentBuilder Builder { get; }
        void Release();
    }
}
