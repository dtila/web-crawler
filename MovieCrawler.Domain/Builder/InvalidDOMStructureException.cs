using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Builder
{
    /// <summary>
    /// Exception throwed when a DOM element is not in the expected location
    /// </summary>
    public class InvalidDOMStructureException : Exception
    {
        public InvalidDOMStructureException(string message)
            : base(message)
        {
        }
    }
}
