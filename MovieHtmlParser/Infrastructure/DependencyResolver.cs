using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Infrastructure;

namespace MovieHtmlParser.Infrastructure
{
    class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        SimpleInjector.Container container;

        public SimpleInjectorDependencyResolver()
        {
            container = new SimpleInjector.Container();
        }

        public void Register<T>(string name = null) where T : class
        {
            container.Register<T>();
        }

        public void Register<T>(T instance, string name = null) where T : class
        {
            container.Register<T>(() => instance);
        }

        public T Resolve<T>(string name) where T : class
        {
            return container.GetInstance<T>();
        }

        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return container.GetAllInstances<T>();
        }
    }
}
