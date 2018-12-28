using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Entity
{
    public class PageContext
    {
        public int Id { get; set; }

        public int LinkId { get; set; }

        public string Context { get; set; }
    }
}
