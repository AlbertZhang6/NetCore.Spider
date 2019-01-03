using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Jwt
{
    internal class RefreshToken
    {
        public string Id
        {
            get;
            set;
        }

        public DateTime Expire
        {
            get;
            set;
        }

        public string JwtId
        {
            get;
            set;
        }
    }
}
