using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Browser;

namespace MovieHtmlParser.Infrastructure
{
    class BrowserFactory : IBrowserFactory
    {
        public IBrowser Create()
        {
            return new EmptyBrowser();
        }

        class EmptyBrowser : IBrowser
        {
            public IBrowserPage Page
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Uri Uri
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public Task Navigate(Uri uri)
            {
                throw new NotImplementedException();
            }
        }
    }
}
