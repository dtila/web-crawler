using ProxyProviderLibrary.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCrawler;

namespace ProxyProviderLibrary.ProxyProviders
{
    class CoolProxyNetProvider : BaseObserver<string>, IProxySource
    {
        const string PageFormat = "http://www.cool-proxy.net/proxies/http_proxy_list/country_code:/port:/anonymous:1/page:{0}";

        private static readonly Regex Regex = new Regex(
@"Base64\.decode\(\""(?<ip>.*?)\""\) #ip
\D+ #skip
(?<port>\d+) #port", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        private static readonly Regex NextPageRegex = new Regex(@"<a href=\""(.*?)\"".*?>Next", RegexOptions.Compiled);

        public string Name
        {
            get { return "cool-proxy.net"; }
        }

        public TimeSpan RefreshTime
        {
            get { return TimeSpan.FromDays(7); }
        }

        public async Task Start(System.Threading.CancellationToken token)
        {
            var requestFactory = DependencyResolver.Resolve<WebCrawler.Infrastructure.IHttpFactory>();
            for(int i=1 ;!token.IsCancellationRequested ; i++)
            {
                var request = requestFactory.Create(new Uri(string.Format(PageFormat, i)));
                var response = await request.GetResponseAsync();
            
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = await reader.ReadToEndAsync();
                    ParsePage(content);

                    var match = NextPageRegex.Match(content);
                    if (match.Success)
                        continue;
                    
                    RaiseOnCompleted();
                    break;
                }
            }
        }

        private void ParsePage(string content)
        {
            foreach (Match match in Regex.Matches(content))
            {
                var ip = Encoding.ASCII.GetString(Convert.FromBase64String(match.Groups["ip"].Value)) + ':' + match.Groups["port"].Value;
                RaiseOnNext(ip);
            }
        }
    }
}
