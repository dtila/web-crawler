using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Data
{
    public interface IContent
    {
        Uri Uri { get; }
    }

    public interface IBrowserContent : IContent
    {
        void Fullfill(IBrowserPageInspectSubscription subscription);
    }
}
