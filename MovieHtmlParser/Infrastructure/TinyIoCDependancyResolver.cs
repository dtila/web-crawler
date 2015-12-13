using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Infrastructure;

namespace MovieHtmlParser.Infrastructure
{
    class TinyIoCDependancyResolver : IDependencyResolver
    {
        private TinyIoC.TinyIoCContainer container;

        public TinyIoCDependancyResolver()
        {
            container = new TinyIoC.TinyIoCContainer();
        }

        public void Register<T>(string name = null) where T : class
        {
            container.Register<T>(name);
        }

        public void Register<T>(T instance, string name = null) where T : class
        {
            container.Register<T>(instance, name);
        }

        public T Resolve<T>(string name) where T : class
        {
            return container.Resolve<T>(name);
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return container.ResolveAll<T>();
        }
    }
}
