using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Entity
{
    public class SpiderSetting
    {
        public int ConnectionTimeout { get; set; }
        public int ThreadSleepTimeWhenQueueIsEmpty
        {
            get
            {
                return 3000;
            }
        }

        public string DownloadFolder { get; set; }

        public int ThreadCount { get; set; }
    }
}
