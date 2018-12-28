using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Model
{
    public class CrawActionModel
    {
        public string Url { get; set; }

        public string PageHtml { get; set; }

        public string Title { get; set; }

        public int Time { get; set; }

        public int PageLength
        {
            get
            {
                if (string.IsNullOrEmpty(PageHtml)) return 0;
                return PageHtml.Length;
            }
        }
    }
}
