using ProxyProviderLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyProviderLibrary.Data
{
    public interface IProxiesStateStorage
    {
        IEnumerable<ProxyInfo> ProxyInfos { get; set; }
        IDictionary<string, ProxyProviderInfo> ProviderInfos { get; set; }

        void Load();
        void Save();
    }

    [Serializable]
    public class ProxyProviderInfo
    {
        public DateTime LastRunDate { get; set; }
    }
}
