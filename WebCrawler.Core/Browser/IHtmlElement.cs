using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Browser
{
    public interface IHtmlElement
    {
        void Click();
        IHtmlElement Query(string selector);
    }
}
