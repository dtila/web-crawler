using ProxyProviderLibrary.Data;
using ProxyProviderLibrary.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyProviderLibrary
{
    public class ProxyServerProvider : IDisposable
    {
        static readonly IProxySource[] AvailableProxies = new IProxySource[] 
        {
            new ProxyProviders.CoolProxyNetProvider(),
            new ProxyProviders.FreeProxyList(),
            new ProxyProviders.ProxyFreeProvider()
        };

        private static readonly Random rand = new Random();
        private IProxiesStateStorage _proxiesStorage;
        private List<ProxyInfo> _proxies;
        private ManualResetEventSlim _emptyEvent;
        private Timer _autoSaveTimer;

        public ProxyServerProvider(IProxiesStateStorage proxiesStorage, TimeSpan autoSaveTimer)
        {
            _emptyEvent = new ManualResetEventSlim();
            _proxiesStorage = proxiesStorage;
            _autoSaveTimer = new Timer(AutoSaveTimerCallback, null, autoSaveTimer, autoSaveTimer);
            
            _proxiesStorage.Load();
            _proxies = new List<ProxyInfo>();
            Add(proxiesStorage.ProxyInfos);

            if (_proxies.Count > 0)
                _emptyEvent.Set();

            foreach (var provider in AvailableProxies)
            {
                var providerState = new ProxyProviderState();
                providerState.Instance = provider;

                ProxyProviderInfo proxyInfo;
                if (proxiesStorage.ProviderInfos.TryGetValue(provider.Name, out proxyInfo) && proxyInfo != null)
                    providerState.LastRunningDate = proxyInfo.LastRunDate;

                providerState.Timer = new Timer(ProxyTimerCallback, providerState, providerState.DueTime, provider.RefreshTime);
                provider.Subscribe(new Observator(this));
            }
        }

        public string GetRandomProxy()
        {
            _emptyEvent.Wait();

            var distribution = new int[_proxies.Count];
            int sum, rand;

            lock (_proxies)
            {
                sum = _proxies[0].WorkingCount;
                distribution[0] = _proxies[0].WorkingCount;
                for (int i = 1; i < _proxies.Count; i++)
                {
                    distribution[i] = distribution[i - 1] + _proxies[i].WorkingCount;
                    sum += _proxies[i].WorkingCount;
                }

                rand = new Random().Next(_proxies[0].WorkingCount, sum);
            }

            int s = distribution[0];
            for (int i = 1; i < distribution.Length; i++)
                if ((s += distribution[i]) >= rand)
                    return _proxies[i - 1].Address;
            
            throw new InvalidOperationException("Unable to retrieve a proxy");
        }

        public void MarkProxyAsFunctional(Uri address)
        {
            lock (_proxies)
            {
                foreach (var proxy in _proxies)
                    if (proxy.Equals(address) && proxy.WorkingCount++ > 30)
                    {
                        proxy.WorkingCount = 10;
                        break;
                    }
            }
        }

        public void Save()
        {
            _proxiesStorage.ProxyInfos = _proxies.ToList();
            _proxiesStorage.Save();
        }

        public void Dispose()
        {
            Save();
        }

        private void AutoSaveTimerCallback(object state)
        {
            try
            {
                Save();
            }
            catch { }
        }

        private void Add(IEnumerable<ProxyInfo> infos)
        {
            lock (_proxies)
            {
                foreach (var proxy in infos)
                {
                    if (_proxies.Exists(pi => pi.Address == proxy.Address))
                        continue;

                    if (proxy.WorkingCount == 0)
                        proxy.WorkingCount = 1;

                    _proxies.Add(proxy);
                    //RaiseOnNext(proxy);
                }

                if (_proxies.Count > 0)
                    _emptyEvent.Set();
            }
        }

        private async void ProxyTimerCallback(object state)
        {
            var providerState = state as ProxyProviderState;

            providerState.Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            try
            {
                var cts = new CancellationTokenSource();
                await providerState.Instance.Start(cts.Token);
                providerState.LastRunningDate = DateTime.Now;

                ProxyProviderInfo providerInfo;
                if (!_proxiesStorage.ProviderInfos.TryGetValue(providerState.Instance.Name, out providerInfo))
                    _proxiesStorage.ProviderInfos.Add(providerState.Instance.Name, providerInfo = new ProxyProviderInfo());
                providerInfo.LastRunDate = providerState.LastRunningDate;

                Save();
            }
            catch (Exception)
            {
            }
            finally
            {
                providerState.Timer.Change(providerState.DueTime, providerState.Instance.RefreshTime);
            }
        }

        class Observator : IObserver<string>
        {
            private ProxyServerProvider _instance;
            private List<ProxyInfo> _collection;

            public Observator(ProxyServerProvider instance)
            {
                _instance = instance;
                _collection = new List<ProxyInfo>();
            }

            public void OnCompleted()
            {
                _instance.Add(_collection);
                _collection.Clear();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(string value)
            {
                _collection.Add(new ProxyInfo { Address = value });
            }
        }


        class ProxyProviderState
        {
            public IProxySource Instance;
            public DateTime LastRunningDate;
            public Timer Timer;

            public TimeSpan DueTime
            {
                get
                {
                    var due = LastRunningDate.Add(Instance.RefreshTime) - DateTime.Now;
                    if (due.TotalMilliseconds < 0)
                        return TimeSpan.Zero;
                    return due;
                }
            }
        }
    }
}
