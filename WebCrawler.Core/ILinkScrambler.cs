using System;
using System.Threading.Tasks;

namespace WebCrawler.Core
{
    public interface ILinkScrambler : IContentCrawler
    {
        Task<Uri> GetLinkAsync();
    }
}
