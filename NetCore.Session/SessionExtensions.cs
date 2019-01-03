using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Extension;
using System;

namespace NetCore.Session
{
    public static class SessionExtensions
    {
        public static ITypedSession AsTyped(this ISession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            return new TypedSession(session);
        }

        public static IServiceCollection AddApiSession(this IServiceCollection services)
        {
            return services.AddApiSession("Infrastructure:Session");
        }

        public static IServiceCollection AddApiSession(this IServiceCollection services, string configureSection)
        {
            if (string.IsNullOrWhiteSpace(configureSection))
            {
                throw new ArgumentException("configureSection");
            }
            IConfiguration val = services.FindConfiguration();
            OptionsConfigurationServiceCollectionExtensions.Configure<SessionOptions>(services, val.GetSection(configureSection));
            return AddApiSessionCore(services);
        }

        public static IServiceCollection AddApiSession(this IServiceCollection services, Action<SessionOptions> configureAction)
        {
            if (configureAction == null)
            {
                throw new ArgumentNullException("configureAction");
            }
            OptionsServiceCollectionExtensions.Configure<SessionOptions>(services, configureAction);
            return AddApiSessionCore(services);
        }

        private static IServiceCollection AddApiSessionCore(IServiceCollection services)
        {
            SessionServiceCollectionExtensions.AddSession(services);
            HttpServiceCollectionExtensions.AddHttpContextAccessor(services);
            ServiceCollectionServiceExtensions.AddTransient<ISessionCreator, SessionCreator>(services);
            ServiceCollectionServiceExtensions.AddSingleton<SessionProtector>(services);
            ServiceCollectionServiceExtensions.AddScoped<SessionScope>(services);
            return services;
        }

        public static IApplicationBuilder UseHeaderSession(this IApplicationBuilder app)
        {
            UseMiddlewareExtensions.UseMiddleware<HeaderSessionMiddleware>(app, Array.Empty<object>());
            return app;
        }
    }

}
