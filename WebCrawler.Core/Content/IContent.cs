using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Content
{
    public interface IContent
    {
        Uri Uri { get; }
    }
}
