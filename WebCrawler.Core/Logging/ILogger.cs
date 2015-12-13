using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Error(string message, Exception ex = null);
        void Info(string message, Exception ex = null);

        void Warning(string message, Exception ex = null);

    }
}
