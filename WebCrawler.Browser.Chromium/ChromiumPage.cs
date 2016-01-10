using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;

namespace WebCrawler.Browser.Chromium
{
    [DebuggerDisplay("ChromiumPage")]
    class ChromiumPage : IBrowserPage
    {
        private CefSharp.IWebBrowser webBrowser;
        private RootHtmlElement root;

        public ChromiumPage(CefSharp.IWebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
            this.root = new RootHtmlElement(webBrowser);
        }

        public IHtmlElement Root
        {
            get { return root; }
        }


        abstract class HtmlElement : IHtmlElement
        {
            private IWebBrowser webBrowser;
            private HtmlElement parent;

            public HtmlElement(IWebBrowser webBrowser, HtmlElement parent)
            {
                this.webBrowser = webBrowser;
                this.parent = parent;
            }

            public abstract string RootElement
            {
                get;
            }

            public void Click()
            {
                var handler = GetEvaluatingPath() + ".click()";
                webBrowser.ExecuteScriptAsync(handler);
            }

            public string GetAttribute(string name)
            {
                var handler = GetEvaluatingPath() + ".getAttribute('" + name + "')";
                var result = webBrowser.EvaluateScriptAsync(handler).Result;
                if (!result.Success)
                    throw new InvalidOperationException(string.Format("Unable to get the attribute with the name {0} on the element", name));
                return result.Result.ToString();
            }

            public IHtmlElement Query(string selector)
            {
                return new DescendantHtmlElement(selector, webBrowser, this);
            }

            private string GetEvaluatingPath()
            {
                var sb = new StringBuilder();
                sb.Append(RootElement);

                for (var current = parent; current != null; current = current.parent)
                {
                    sb.Insert(0, '.');
                    sb.Insert(0, current.RootElement);
                }

                return sb.ToString();
            }

            public override string ToString()
            {
                return GetEvaluatingPath();
            }
        }

        class RootHtmlElement : HtmlElement
        {
            public RootHtmlElement(IWebBrowser webBrowser) : base(webBrowser, null)
            {
            }

            public override string RootElement
            {
                get { return "document"; }
            }
        }

        class DescendantHtmlElement : HtmlElement
        {
            private string selector;

            public DescendantHtmlElement(string selector, IWebBrowser webBrowser, HtmlElement parent) 
                : base(webBrowser, parent)
            {
                this.selector = selector;
            }

            public override string RootElement
            {
                get { return "querySelector('" + selector + "')"; }
            }
        }
    }
}
