using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTA
{
    public class MovieProvider
    {
        public string Name { get; set; }
        public ProviderStatus Status { get; set; }
    }

    public enum ProviderStatus
    {
        Running,
        Enabled,
        Disabled
    }
}
