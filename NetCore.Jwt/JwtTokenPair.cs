using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Jwt
{
    public class JwtTokenPair
    {
        [JsonProperty("auth_token")]
        public string AuthToken
        {
            get;
            set;
        }

        [JsonProperty("refresh_token")]
        public string RefreshToken
        {
            get;
            set;
        }

        [JsonProperty("iat")]
        public DateTime? IssuedAt
        {
            get;
            set;
        }

        [JsonProperty("exp")]
        public DateTime? ExpirationTime
        {
            get;
            set;
        }
    }
}
