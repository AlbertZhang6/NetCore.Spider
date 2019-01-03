using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.WebApi
{
    internal class QueryStringAuthTokenMiddleware
    {
        private const string AuthorizationKey = "Authorization";

        private readonly RequestDelegate next;

        public QueryStringAuthTokenMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!((IDictionary<string, StringValues>)context.Request.Headers).ContainsKey("Authorization") && context.Request.Query.ContainsKey("Authorization"))
            {
                StringValues auth = context.Request.Query["Authorization"];
                string authString = ((object)auth).ToString();
                string bearer = "Bearer ";
                ((IDictionary<string, StringValues>)context.Request.Headers).Add("Authorization", new StringValues(authString.StartsWith(bearer, StringComparison.OrdinalIgnoreCase) ? authString : (bearer + authString)));
                auth = default(StringValues);
            }
            if (!((IDictionary<string, StringValues>)context.Request.Headers).ContainsKey("Api-Session") && context.Request.Query.ContainsKey("Api-Session"))
            {
                StringValues session = context.Request.Query["Api-Session"];
                ((IDictionary<string, StringValues>)context.Request.Headers).Add("Api-Session", session);
            }
            await next.Invoke(context);
        }
    }

}
