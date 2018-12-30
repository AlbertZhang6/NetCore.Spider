using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Spider.Common
{
    public class HttpHelper
    {

        public static string GetHtmlSourceCode(string uri)
        {
            var httpClient = new HttpClient();
            try
            {
                var htmlSource = httpClient.GetStringAsync(uri).Result;
                return htmlSource;
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{nameof(HttpRequestException)}: {e.Message}");
                return null;
            }
        }

        public static async Task<string> GetHtmlSourceCodeAsync(string uri)
        {
            var httpClient = new HttpClient();
            try
            {
                return await httpClient.GetStringAsync(uri);
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public static string GetHtmlByUrl(string url)
        {
            try
            {
                //var htmlContext = new PageSource() { Url = url };
                Stopwatch timer = new Stopwatch();
                timer.Start();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = SpiderSettings.ConnectionTimeout;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    if (response.ContentType == "text/html")
                    {
                        using (Stream streamReceive = response.GetResponseStream())
                        {
                            Encoding encoding = Encoding.GetEncoding("UTF-8");
                            using (StreamReader streamReader = new StreamReader(streamReceive, encoding))
                            {
                                //htmlContext.Context = streamReader.ReadToEnd();
                                timer.Stop();
                                //htmlContext.TimeSpend = timer.Elapsed.Seconds;
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void DownLoadHtml(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            // 设置超时以避免耗费不必要的时间等待响应缓慢的服务器或尺寸过大的网页.
            req.Timeout = SpiderSettings.ConnectionTimeout * 1000;
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                string contentType = response.ContentType;
                if (contentType != "text/html") return;

                byte[] buffer = ReadInstreamIntoMemory(response.GetResponseStream());

                if (!Directory.Exists(SpiderSettings.DownloadFolder))
                {
                    Directory.CreateDirectory(SpiderSettings.DownloadFolder);
                }

                string extension = GetExtensionByMimeType(contentType);
                string md5 = new Guid().ToString();//Utility.Hash(url);
                string fileName = Path.Combine(SpiderSettings.DownloadFolder, md5 + "." + extension);
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private string GetExtensionByMimeType(string mimeType)
        {
            int pos;
            if ((pos = mimeType.IndexOf('/')) != -1)
            {
                return mimeType.Substring(pos + 1);
            }
            return string.Empty;
        }

        private byte[] ReadInstreamIntoMemory(Stream stream)
        {
            int bufferSize = 16384;
            byte[] buffer = new byte[bufferSize];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int numBytesRead = stream.Read(buffer, 0, bufferSize);
                    if (numBytesRead <= 0) break;
                    ms.Write(buffer, 0, numBytesRead);
                }
                return ms.ToArray();
            }
        }


        /// <summary>
        /// 代理IP池子
        /// </summary>
        //private  AvailableProxy availableProxy = AvailableProxyHepler.GetAvailableProxy();

        //public  HttpClient Client { get; } = new HttpClient();

        ///// <summary>
        ///// 通过HTTP获取HTML（默认不使用代理）
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="isUseProxy"></param>
        ///// <returns></returns>
        //public  string GetHTMLByURL(string url, bool isUseProxy = false)
        //{
        //    ProxyInfo proxyInfo = null;
        //    try
        //    {
        //        System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
        //        CrawlerProxyInfo crawlerProxyInfo = null;
        //        //测试中发现使用代理会跳转到中转页，解决方案暂时不明确，先屏蔽代理
        //        if (url.Contains(SoureceDomainConsts.BTdytt520) && isUseProxy)
        //        {
        //            var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
        //            proxyInfo = availableProxy.Btdytt520[index];
        //            crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.Ip}:{proxyInfo.Port}");

        //        }
        //        else if (url.Contains(SoureceDomainConsts.Dy2018Domain) && isUseProxy)
        //        {
        //            var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
        //            proxyInfo = availableProxy.Dy2018[index];
        //            crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.Ip}:{proxyInfo.Port}");
        //        }

        //        wRequest.Proxy = crawlerProxyInfo;
        //        wRequest.ContentType = "text/html; charset=gb2312";

        //        wRequest.Method = "get";
        //        wRequest.UseDefaultCredentials = true;
        //        // Get the response instance.
        //        var task = wRequest.GetResponseAsync();
        //        System.Net.WebResponse wResp = task.Result;
        //        System.IO.Stream respStream = wResp.GetResponseStream();
        //        using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding("GB2312")))
        //        {
        //            return reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string proxyIP = isUseProxy ? proxyInfo.Ip : "No Use Proxy";
        //        //LogHelper.Error("GetHTMLByURL Exception", ex, $"URL:{url},Proxy:{proxyIP}");
        //        return string.Empty;
        //    }
        //}


        ///// <summary>
        ///// 通过HTTP获取HTML（默认不使用代理）
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="isUseProxy"></param>
        ///// <returns></returns>
        //public  string GetHTML(HttpWebRequest wRequest, bool isUseProxy = false)
        //{
        //    ProxyInfo proxyInfo = null;
        //    try
        //    {

        //        CrawlerProxyInfo crawlerProxyInfo = null;
        //        //测试中发现使用代理会跳转到中转页，解决方案暂时不明确，先屏蔽代理
        //        if (wRequest.RequestUri.Host.Contains(SoureceDomainConsts.BTdytt520) && isUseProxy)
        //        {
        //            var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
        //            proxyInfo = availableProxy.Btdytt520[index];
        //            crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.Ip}:{proxyInfo.Port}");

        //        }
        //        else if (wRequest.RequestUri.Host.Contains(SoureceDomainConsts.Dy2018Domain) && isUseProxy)
        //        {
        //            var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
        //            proxyInfo = availableProxy.Dy2018[index];
        //            crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.Ip}:{proxyInfo.Port}");
        //        }

        //        wRequest.Proxy = crawlerProxyInfo;
        //        wRequest.ContentType = "text/html; charset=gb2312";

        //        wRequest.Method = "get";
        //        wRequest.UseDefaultCredentials = true;
        //        // Get the response instance.
        //        var task = wRequest.GetResponseAsync();
        //        System.Net.WebResponse wResp = task.Result;
        //        System.IO.Stream respStream = wResp.GetResponseStream();
        //        using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding("GB2312")))
        //        {
        //            return reader.ReadToEnd();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string proxyIP = isUseProxy ? proxyInfo.Ip : "No Use Proxy";
        //        LogHelper.Error("GetHTMLByURL Exception", ex, $"URL:{wRequest.RequestUri.Host},Proxy:{proxyIP}");
        //        return string.Empty;
        //    }
        //}


    }
}
