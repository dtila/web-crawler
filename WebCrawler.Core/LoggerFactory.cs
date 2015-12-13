using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;
using WebCrawler.Infrastructure;
using WebCrawler.Logging;

namespace WebCrawler
{
    public static class LoggerFactory
    {
        public static ILogger Create(string name)
        {
            return DependencyResolver.Resolve<ILoggerFactory>().Create(name);
        }

        public static ILogger Create(Type type)
        {
            return DependencyResolver.Resolve<ILoggerFactory>().Create(type);
        }
        public static ILogger Create(string name, Type type)
        {
            return DependencyResolver.Resolve<ILoggerFactory>().Create(name, type);
        }
    }
}
