using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCore.Extension;
using System;
using System.IO;

namespace NetCore.Security
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurityComponent(this IServiceCollection services)
        {
            return services.AddSecurityComponent("Infrastructure:Security");
        }

        public static IServiceCollection AddSecurityComponent(this IServiceCollection services, string configureSection)
        {
            if (string.IsNullOrWhiteSpace(configureSection))
            {
                throw new ArgumentException("configureSection");
            }
            IConfiguration val = services.FindConfiguration();
            SecurityOptions securityOptions = new SecurityOptions();
            ConfigurationBinder.Bind(val, configureSection, (object)securityOptions);
            OptionsConfigurationServiceCollectionExtensions.Configure<SecurityOptions>(services, val.GetSection(configureSection));
            return AddSecurityComponentCore(services, securityOptions);
        }

        public static IServiceCollection AddSecurityComponent(this IServiceCollection services, Action<SecurityOptions> configureAction)
        {
            if (configureAction == null)
            {
                throw new ArgumentNullException("configureAction");
            }
            SecurityOptions securityOptions = new SecurityOptions();
            configureAction(securityOptions);
            OptionsServiceCollectionExtensions.Configure<SecurityOptions>(services, configureAction);
            return AddSecurityComponentCore(services, securityOptions);
        }

        private static IServiceCollection AddSecurityComponentCore(IServiceCollection services, SecurityOptions options)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(options.KeysRingDirectory);
            DataProtectionBuilderExtensions.DisableAutomaticKeyGeneration(DataProtectionBuilderExtensions.PersistKeysToFileSystem(DataProtectionBuilderExtensions.SetApplicationName(DataProtectionServiceCollectionExtensions.AddDataProtection(services), options.ApplicationName), directoryInfo));
            SecurityEncryptor securityEncryptor = new SecurityEncryptor(Options.Create<SecurityOptions>(options));
            ServiceCollectionServiceExtensions.AddSingleton<ISecurityEncryptor>(services, (ISecurityEncryptor)securityEncryptor);
            SecurityEncryptorFactory.Configure(securityEncryptor);
            return services;
        }
    }

}
