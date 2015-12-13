using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyProviderLibrary
{
    interface IProxiesStateStorage
    {
        IEnumerable<ProxyInfo> Load();
        void Save(IEnumerable<ProxyInfo> proxies);

        DateTime GetLastRunDate(IProxiesProvider provider);
        void SetLastRunDate(IProxiesProvider provider, DateTime date);
    }
}
