using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{
    public class SecurityOptions
    {
        public string ApplicationName
        {
            get;
            set;
        }

        public string SecureKey
        {
            get;
            set;
        }

        public string KeysRingDirectory
        {
            get;
            set;
        }
    }

}
