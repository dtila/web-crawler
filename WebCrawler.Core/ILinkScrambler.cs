using System;
using System.Threading.Tasks;

namespace WebCrawler.Core
{
    public interface ILinkScrambler : ICrawler
    {
        Task<Uri> GetLinkAsync();
    }
}
