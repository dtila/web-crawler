using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Content.Builder
{
    public interface IContentBuilder
    {
        void Enqueue(IBrowserContent content);

        Task Enqueue(ILinkUnscrambler scrambler);
    }

    public interface IContentBuilder<in TType> where TType : IContent
    {
        Task Enqueue(TType type);
    }
}
