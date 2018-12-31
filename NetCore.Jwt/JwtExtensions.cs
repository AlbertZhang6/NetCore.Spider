using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetCore.Extension;
using NetCore.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Jwt
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            return services.AddJwt("Infrastructure:Jwt");
        }

        public static IServiceCollection AddJwt(this IServiceCollection services, string configureSection)
        {
            if (string.IsNullOrWhiteSpace(configureSection))
            {
                throw new ArgumentException("configureSection");
            }
            IConfiguration val = services.FindConfiguration();
            JwtOptions jwtOptions = new JwtOptions();
            ConfigurationBinder.Bind(val, configureSection, (object)jwtOptions);
            OptionsConfigurationServiceCollectionExtensions.Configure<JwtOptions>(services, val.GetSection(configureSection));
            return AddJwtCore(services, jwtOptions);
        }

        private static IServiceCollection AddJwtCore(IServiceCollection services, JwtOptions options)
        {
            JwtBearerExtensions.AddJwtBearer(AuthenticationServiceCollectionExtensions.AddAuthentication(services, "JWT"), "JWT", (JwtBearerOptions p) =>
           {
               p.RequireHttpsMetadata = false;
               p.TokenValidationParameters.IssuerSigningKey = (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.PrivateKey)));
               p.TokenValidationParameters.ClockSkew = options.ClockSkew;
               p.TokenValidationParameters.ValidIssuer = options.Issuer;
               p.TokenValidationParameters.ValidAudiences = ((IEnumerable<string>)new string[1]
               {
                options.Audience
               });
               p.TokenValidationParameters.RequireSignedTokens = (true);
               p.TokenValidationParameters.RequireExpirationTime = (true);
               p.TokenValidationParameters.AuthenticationType = "JWT";
               p.TokenValidationParameters.NameClaimType = options.NameClaim;
               p.TokenValidationParameters.RoleClaimType = options.RoleClaim;
               p.TokenValidationParameters.ValidateIssuerSigningKey = true;
               p.TokenValidationParameters.ValidateIssuer = true;
               p.TokenValidationParameters.ValidateAudience = true;
               p.TokenValidationParameters.ValidateLifetime = true;
           });
            ServiceCollectionServiceExtensions.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(services);
            services.AddSecurityComponent();
            return services;
        }

        public static IServiceCollection AddJwt(this IServiceCollection services, Action<JwtOptions> configureAction)
        {
            if (configureAction == null)
            {
                throw new ArgumentNullException("configureAction");
            }
            JwtOptions jwtOptions = new JwtOptions();
            configureAction(jwtOptions);
            OptionsServiceCollectionExtensions.Configure<JwtOptions>(services, configureAction);
            return AddJwtCore(services, jwtOptions);
        }

        public static IApplicationBuilder UseJwtAuthorize(this IApplicationBuilder app)
        {
            UseMiddlewareExtensions.UseMiddleware<JwtAuthorizeMiddleware>(app, Array.Empty<object>());
            return app;
        }
    }
}
