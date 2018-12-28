using NetCore.Model.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Spider.Common.Helpers
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
                throw e;
            }
        }

        public static async Task<string> GetHtmlSourceAsync(string uri)
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
                req.Timeout = 10000;//SpiderSettings.ConnectionTimeout;
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


        public void DownLoadHtml(string url, SpiderSetting spiderSetting)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            // 设置超时以避免耗费不必要的时间等待响应缓慢的服务器或尺寸过大的网页.
            req.Timeout = spiderSetting.ConnectionTimeout * 1000;
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                string contentType = response.ContentType;
                if (contentType != "text/html") return;

                byte[] buffer = ReadInstreamIntoMemory(response.GetResponseStream());

                if (!Directory.Exists(spiderSetting.DownloadFolder))
                {
                    Directory.CreateDirectory(spiderSetting.DownloadFolder);
                }

                string extension = GetExtensionByMimeType(contentType);
                string md5 = new Guid().ToString();//Utility.Hash(url);
                string fileName = Path.Combine(spiderSetting.DownloadFolder, md5 + "." + extension);
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
    }
}
