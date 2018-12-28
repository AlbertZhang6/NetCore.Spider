using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Entity
{
    public class Link
    {
        public int Id { get; set; }

        public int LinkTypeCode { get; set; }

        public int CatalogType { get; set; }

        public string Url { get; set; }

        public string PageTitle { get; set; }

        public ICollection<PageContext> PageContexts { get; set; }

        //public ICollection<CrawHistory> CrawHistories { get; set; }

    }
}
