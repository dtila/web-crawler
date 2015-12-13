using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyProviderLibrary.Data
{
    interface IProxySource : IObservable<string>
    {
        string Name { get; }
        TimeSpan RefreshTime { get; }
        Task Start(CancellationToken token);
    }
}
