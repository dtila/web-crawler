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
        void Enqueue(IBrowserContent content);

        Task Enqueue(ILinkScrambler scrambler);
    }

    public interface IContentBuilder<in TType> where TType : IContent
    {
        Task Enqueue(TType type);
    }
}
