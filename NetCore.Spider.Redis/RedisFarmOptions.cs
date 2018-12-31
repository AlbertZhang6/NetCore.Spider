using System.Collections.Generic;

namespace NetCore.Redis
{
    public class RedisFarmOptions
    {
        public RedisFarmMode Mode
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string ServiceName
        {
            get;
            set;
        }

        public List<string> Servers
        {
            get;
            set;
        }

        public ushort Database
        {
            get;
            set;
        }
    }
}
