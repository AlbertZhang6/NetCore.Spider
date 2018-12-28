using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Entity
{
    public class CrawHistory
    {
        public int Id { get; set; }

        public int LinkStepCode { get; set; }

        public string Url { get; set; }
    }
}
