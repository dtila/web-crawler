using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTA
{
    public class MovieStream
    {
        public int Id { get; set; }
        public string Video { get; set; }
        public byte Resolution { get; set; }
    }
}
