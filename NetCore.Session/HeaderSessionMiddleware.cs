using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCore.Session
{
    internal class HeaderSessionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ISessionStore store;
        private readonly SessionProtector protector;
        private readonly ILogger<HeaderSessionMiddleware> logger;
        private readonly SessionOptions options;

        public HeaderSessionMiddleware(RequestDelegate next, ISessionStore store,
            SessionProtector protector, ILogger<HeaderSessionMiddleware> logger,
             IOptions<SessionOptions> options)
        {
            this.next = next;
            this.store = store;
            this.protector = protector;
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task Invoke(HttpContext context, SessionScope scope)
        {
            if (((IDictionary<string, StringValues>)context.Request.Headers).TryGetValue("Api-Session", out StringValues value))
            {
                string sessionKey = protector.Unprotect(value);
                if (!string.IsNullOrEmpty(sessionKey))
                {
                    ISession session = store.Create(sessionKey, options.IdleTimeout, options.IOTimeout, (Func<bool>)scope.TryEstablish, false);
                    SessionFeature feature = new SessionFeature()
                    {
                        Session = session
                    };
                    context.Features.Set<ISessionFeature>(feature);
                    scope.Session = session;
                    scope.Key = value;
                }
            }
            try
            {
                await next(context);
            }
            finally
            {
                context.Features.Set<ISessionFeature>(null);
                try
                {
                    if (scope.Session != null)
                    {
                        //session has been already created by middleware or ISessionManager.Create
                        try
                        {
                            await scope.Session.CommitAsync(context.RequestAborted);
                        }
                        catch (OperationCanceledException)
                        {
                            logger.LogWarning("committing the session was canceled.");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "an error occurred during committing the session.");
                        }
                    }
                }
                finally
                {
                    scope.Complete();
                }
            }
        }
    }
}
