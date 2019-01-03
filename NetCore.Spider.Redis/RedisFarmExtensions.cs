using NetCore.Extension;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NetCore.Redis
{
    public static class RedisFarmExtensions
    {
        public static IServiceCollection AddRedisFarmCache(this IServiceCollection services)
        {
            return services.AddRedisFarmCache("Infrastructure:Redis");
        }

        public static IServiceCollection AddRedisFarmCache(this IServiceCollection services, string configureSection)
        {
            if (string.IsNullOrWhiteSpace(configureSection))
            {
                throw new ArgumentException("configureSection");
            }
            IConfiguration configuration = services.FindConfiguration();
            OptionsConfigurationServiceCollectionExtensions.Configure<RedisFarmOptions>(services, configuration.GetSection(configureSection));
            return AddRedisFarmCacheCore(services);
        }

        public static IServiceCollection AddRedisFarmCache(this IServiceCollection services, Action<RedisFarmOptions> configureAction)
        {
            if (configureAction == null)
            {
                throw new ArgumentNullException("configureAction");
            }
            OptionsServiceCollectionExtensions.Configure<RedisFarmOptions>(services, configureAction);
            return AddRedisFarmCacheCore(services);
        }

        private static IServiceCollection AddRedisFarmCacheCore(IServiceCollection services)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IDistributedCache, RedisFarm>(services);
            return services;
        }
    }
}
