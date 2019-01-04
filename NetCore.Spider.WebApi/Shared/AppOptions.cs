using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.WebApi.Shared
{
    public class AppOptions
    {
        public ServicesOptions Services { get; set; }
    }

    public class ServicesOptions
    {
        public ServiceOptions Business { get; set; }
    }

    public class ServiceOptions
    {
        public string Address { get; set; }
    }
}
