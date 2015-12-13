using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Core
{
    public interface ICrawler
    {
        Uri Uri { get; }
    }
}
