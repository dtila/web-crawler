using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core;

namespace WebCrawler.Data
{
    public interface IContentBuilder
    {
        //Task Enqueue(Uri uri);

        void Enqueue(IBrowserContent content);

        Task Enqueue(ILinkScrambler scrambler);
    }
}
