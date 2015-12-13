using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Infrastructure;

namespace WebCrawler
{
    public class DependencyResolver
    {
        private static IDependencyResolver current;
        public static IDependencyResolver Current
        {
            get
            {
                if (current == null)
                    throw new InvalidOperationException("No dependency injection resolver is set");
                return current;
            }
            set { current = value; }
        }

        public static T Resolve<T>(string name = null) where T : class
        {
            return Current.Resolve<T>(name);
        }
        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Current.ResolveAll<T>();
        }

        public static void Register<T>(string name = null) where T : class
        {
            Current.Register<T>(name);
        }
        public static void Register<T>(T instance, string name = null) where T : class
        {
            Current.Register<T>(instance, name);
        }
    }
}
