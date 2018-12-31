using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Jwt
{
    internal class JwtAuthorizeMiddleware
    {
        private readonly RequestDelegate next;

        private IDistributedCache cache;

        private static readonly ClaimsIdentity AuthorizeIdentity = new ClaimsIdentity(new Claim[1]
        {
        new Claim("__jwt_authorize", "!PASS", "http://www.w3.org/2001/XMLSchema#string")
        });

        public JwtAuthorizeMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            Claim jti = context.User.FindFirst("jti");
            if (jti != null && !string.IsNullOrEmpty(jti.Value) && await cache.GetAsync(jti.Value, default(CancellationToken)) == null)
            {
                context.User.AddIdentity(AuthorizeIdentity);
            }
            await next.Invoke(context);
        }
    }

}
