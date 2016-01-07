using System;
using System.Threading.Tasks;

namespace WebCrawler.Content.Builder
{
    public interface ILinkUnscrambler : IContentCrawler
    {
        Task<Uri> GetLinkAsync();
    }
}
