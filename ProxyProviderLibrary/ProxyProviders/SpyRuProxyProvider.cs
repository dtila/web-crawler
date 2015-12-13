using mshtml;
using ProxyProviderLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyProviderLibrary.ProxyProviders
{
    public class SpyRuProxyProvider : IProxySource
    {
        public string Name
        {
            get { return "spu.ru"; }
        }

        public TimeSpan RefreshTime
        {
            get { return TimeSpan.FromDays(1); }
        }

        public Task Start(System.Threading.CancellationToken token)
        {
            var tcs = new TaskCompletionSource<string>();
            SHDocVw.InternetExplorer ie = new SHDocVw.InternetExplorer();
            ie.Visible = true;
            ie.DocumentComplete += (object pDisp, ref object URL) =>
            {
                ParseProxies(ie.Document as mshtml.HTMLDocument);
            };
            ie.Navigate("http://spys.ru/en/non-anonymous-proxy-list/");
            return tcs.Task;
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return null;
        }

        private IEnumerable<string> ParseProxies(mshtml.HTMLDocument document)
        {
            foreach (var column in document.getElementsByTagName("td").OfType<IHTMLElement>())
            {

            }
            /*foreach (IHTMLElement element in document.body.children)
            {
                if (element.tagName == "table")
                {

                }
            }*/
            return null;
        }
    }
}
