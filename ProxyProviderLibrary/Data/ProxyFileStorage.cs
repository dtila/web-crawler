using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyProviderLibrary.Data
{
    using Model;
    using System.Runtime.Serialization.Formatters.Binary;

    public class ProxyFileStorage : IProxiesStateStorage
    {
        private string _proxyListFileName;
        private string _proxyInfoFileName;
        private IFormatter _serializer;

        public IEnumerable<ProxyInfo> ProxyInfos { get; set; }
        public IDictionary<string, ProxyProviderInfo> ProviderInfos { get; set; }

        public ProxyFileStorage(string proxyListFileName, string proxyInfoFileName)
        {
            _proxyListFileName = proxyListFileName;
            _proxyInfoFileName = proxyInfoFileName;
            ProxyInfos = new List<ProxyInfo>();
            ProviderInfos = new Dictionary<string, ProxyProviderInfo>();
            _serializer = new BinaryFormatter();
        }

        public void Load()
        {
            lock (_serializer)
            {
                try
                {
                    using (var stream = File.OpenRead(_proxyListFileName))
                    {
                        var result = _serializer.Deserialize(stream) as IEnumerable<ProxyInfo>;
                        if (result != null)
                            ProxyInfos = result;
                    }
                }
                catch (FileNotFoundException) { }

                try
                {
                    using (var stream = File.OpenRead(_proxyInfoFileName))
                    {
                        var enumerable = (_serializer.Deserialize(stream) as IEnumerable<KeyValuePair<string, ProxyProviderInfo>>);
                        if (enumerable != null)
                            ProviderInfos = enumerable.ToDictionary(li => li.Key, li => li.Value);
                    }
                }
                catch (FileNotFoundException) { }
            }
        }

        public void Save()
        {
            lock (_serializer)
            {
                if (ProxyInfos != null)
                {
                    using (var stream = File.OpenWrite(_proxyListFileName))
                        _serializer.Serialize(stream, ProxyInfos);
                }

                if (ProviderInfos != null)
                {
                    using (var stream = File.OpenWrite(_proxyInfoFileName))
                        _serializer.Serialize(stream, ProviderInfos.ToList());
                }
            }
        }
    }
}
