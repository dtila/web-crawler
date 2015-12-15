using MovieCrawler.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core;

namespace MovieCrawler.Domain
{
    public interface IMovieCrawler : IContentCrawler
    {
        void AppendTo(MovieBuilder builder, BrowserPageInspectSubscription subscription);
        //void BuildTo(MovieBuilder builder);
    }
}
