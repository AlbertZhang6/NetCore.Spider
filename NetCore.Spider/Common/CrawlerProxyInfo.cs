using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetCore.Spider.Common
{
    public class CrawlerProxyInfo : IWebProxy
    {
        public CrawlerProxyInfo(string proxyUri)
            : this(new Uri(proxyUri))
        {
        }

        public CrawlerProxyInfo(Uri proxyUri)
        {
            ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination) => ProxyUri;

        public bool IsBypassed(Uri host) => false;/* Proxy all requests */
    }
}
