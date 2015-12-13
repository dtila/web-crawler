using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyProviderLibrary
{
    public interface IProxiesProvider : IObservable<ProxyInfo>
    {
        TimeSpan RefreshTime { get; }
        Task Start(CancellationToken token);
    }

    public class ProxyEventArgs : EventArgs
    {
        public ProxyInfo Proxy { get; private set; }
        
        public ProxyEventArgs(ProxyInfo proxy)
        {
            Proxy = proxy;
        }
    }
}
