using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Logging;

namespace WebCrawler.Infrastructure
{
    public interface ILoggerFactory
    {
        ILogger Create(string name);
        ILogger Create(Type type);
        ILogger Create(string name, Type type);
    }
}
