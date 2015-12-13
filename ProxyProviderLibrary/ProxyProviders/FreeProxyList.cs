using ProxyProviderLibrary.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCrawler;

namespace ProxyProviderLibrary.ProxyProviders
{
    class FreeProxyList : BaseObserver<string>, IProxySource
    {
        private static readonly Regex Regex = new Regex(
@"(?<ip>\d+\.\d+\.\d+\.\d+) #ip
.*?
(?<port>\d+)
.*?
>(?<country_code>\w+)<
.*?
>(?<country>\w+)<
.*?
>(?<type>\w+)<", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public string Name
        {
            get { return "free-proxy-list.net"; }
        }

        public TimeSpan RefreshTime
        {
            get { return TimeSpan.FromDays(3); }
        }

        public async Task Start(System.Threading.CancellationToken token)
        {
            var requestFactory = DependencyResolver.Resolve<WebCrawler.Infrastructure.IHttpFactory>();

            var request = requestFactory.Create(new Uri("http://free-proxy-list.net/anonymous-proxy.html"));
            var response = await request.GetResponseAsync();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var content = await sr.ReadToEndAsync();
                foreach (Match match in Regex.Matches(content))
                {
                    var ip = match.Groups["ip"].Value + ':' + match.Groups["port"].Value;
                    RaiseOnNext(ip);
                }

                RaiseOnCompleted();
            }
        }
    }
}
