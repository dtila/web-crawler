using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyProviderLibrary.Model
{
    [Serializable, DebuggerDisplay("{WorkingCount} - {Address} ")]
    public class ProxyInfo
    {
        public string Address { get; set; }
        public byte WorkingCount { get; set; }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public bool Equals(Uri uri)
        {
            return String.Concat(uri.Host, ':', uri.Port).Equals(Address);
        }

        public override bool Equals(object obj)
        {
            var pi = obj as ProxyInfo;
            if (pi == null)
                return false;
            return pi.Address == Address;
        }
    }

    class ProxyInfoComparer : IComparer<ProxyInfo>
    {
        public int Compare(ProxyInfo x, ProxyInfo y)
        {
            if (x.WorkingCount == y.WorkingCount)
                return 0;
            if (x.WorkingCount < y.WorkingCount)
                return -1;
            return 1;
        }
    }

}
