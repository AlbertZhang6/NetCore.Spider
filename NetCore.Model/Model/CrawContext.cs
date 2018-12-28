using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Model
{
    public class CrawContentBase
    {
        public string SourceLink { get; set; }

        public string PageTitle { get; set; }

        public int Size { get; set; }

        public int Time { get; set; }

        public int TotalPageCount { get; set; }
    }
}
