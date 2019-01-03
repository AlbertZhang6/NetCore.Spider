using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;

namespace NetCore.Jwt
{
    public class JwtAuthorizeFilter: AuthorizeFilter
    {
        public JwtAuthorizeFilter()
        : base(BuildPolicy())
        {
        }

        private static AuthorizationPolicy BuildPolicy()
        {
            //var policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .RequireAssertion(new Func<AuthorizationHandlerContext, Task<bool>>(JwtBlacklistPolicy.Authorize))
            //        .Build();
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            return new AuthorizationPolicyBuilder(Array.Empty<string>()).RequireAuthenticatedUser().RequireClaim("__jwt_authorize", new string[1]
            {
            "!PASS"
            }).Build();
        }
    }
}
