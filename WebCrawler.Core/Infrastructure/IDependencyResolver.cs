using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Infrastructure
{
    public interface IDependencyResolver
    {
        IEnumerable<T> ResolveAll<T>() where T : class;
        T Resolve<T>(string name) where T : class;
        void Register<T>(string name = null) where T : class;
        void Register<T>(T instance, string name = null) where T : class;
    }
}
