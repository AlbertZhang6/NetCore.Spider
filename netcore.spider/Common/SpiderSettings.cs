using System.IO;
using System.Reflection;

namespace NetCore.Spider.Common
{
    public  class SpiderSettings
    {

        private  string ConfigurationFilePath;

         SpiderSettings()
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ConfigurationFilePath = Path.Combine(folder, "config.ini");
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public static int ConnectionTimeout
        {
            get
            {
                return 5000;
            }
            //set
            //{
            //    SetValue("ConnectionTimeout", value);
            //}
        }

        public  int ThreadSleepTimeWhenQueueIsEmpty
        {
            get
            {
                return 3000; 
            }
            //set
            //{
            //    SetValue("ThreadSleepTimeWhenQueueIsEmpty", value);
            //}
        }

        public static  string DownloadFolder
        {
            get
            {
                string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(folder, "DownloadFolder");// Convert.ToString(GetValue("DownloadFolder", "download"));
            }
            //set
            //{
            //    SetValue("DownloadFolder", value);
            //}
        }

        public  int ThreadCount
        {
            get
            {
                return 10;
                //return Convert.ToInt32(GetValue("ThreadCount", 10));
            }
        }

        public  string HtmlParse_Title
        {
            get
            {
                return "Title";
;            }
        }

        public  string HtmlParse_Link
        {
            get
            {
                return "link";
            }
        }
    }
}
