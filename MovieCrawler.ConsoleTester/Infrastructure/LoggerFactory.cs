using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Infrastructure;
using WebCrawler.Logging;

namespace MovieHtmlParser.Infrastructure
{
    class LoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type)
        {
            return new ConsoleLogger();
        }

        public ILogger Create(string name)
        {
            return new ConsoleLogger();
        }

        public ILogger Create(string name, Type type)
        {
            return new ConsoleLogger();
        }


        class ConsoleLogger : ILogger
        {
            public void Debug(string message)
            {
                Console.Write("DEBUG: ");
                Console.WriteLine(message);
            }

            public void Error(string message, Exception ex = null)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(message);
            }

            public void Info(string message, Exception ex = null)
            {
                Console.Write("INFO: ");
                Console.WriteLine(message);
            }

            public void Warning(string message, Exception ex = null)
            {
                Console.Write("WARN: ");
                Console.WriteLine(message);
            }
        }
    }
}
