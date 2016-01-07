using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;

namespace WebCrawler.Content
{
    public interface IBrowserContent : IContent
    {
        void Fullfill(IBrowserPageInspectSubscription subscription);
    }
}
