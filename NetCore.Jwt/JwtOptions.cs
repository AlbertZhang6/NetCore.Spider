using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Jwt
{
    public class JwtOptions
    {
        public string Issuer => "https://www.aspiraconnect.com";

        public string Audience => "fish wildlife.gov";

        public string PrivateKey
        {
            get;
            set;
        }

        public TimeSpan ValidTime
        {
            get;
            set;
        }

        public TimeSpan ClockSkew
        {
            get;
            set;
        }

        public string NameClaim => "jwt_name";

        public string RoleClaim => "jwt_role";
    }

}
