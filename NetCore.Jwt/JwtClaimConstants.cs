using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Jwt
{
    public static class JwtClaimConstants
    {
        public const string NameClaim = "jwt_name";

        public const string RoleClaim = "jwt_role";

        internal const string AuthorizeClaimType = "__jwt_authorize";

        internal const string AuthorizeClaimValue = "!PASS";
    }

}
