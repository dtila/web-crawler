using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieCrawler.Domain
{
    public class InvalidParseElementException : Exception
    {
        public InvalidParseElementException()
            : base("")
        {

        }

        public InvalidParseElementException(string xpath)
            : base(string.Format("{0}", xpath))
        {

        }
    }
}
