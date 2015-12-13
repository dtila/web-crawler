using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Browser
{
    public interface IBrowser
    {
        /// <summary>
        /// The current page
        /// </summary>
        Uri Uri { get; }

        IBrowserPage Page { get; }

        /// <summary>
        /// Navigate to the specified address
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task Navigate(Uri uri);
    }
}
