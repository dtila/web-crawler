using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Builder
{
    public class InvalidElementValueException : Exception
    {
        public InvalidElementValueException()
            : base("")
        {

        }

        public InvalidElementValueException(string xpath)
            : base(xpath)
        {

        }
    }
}
