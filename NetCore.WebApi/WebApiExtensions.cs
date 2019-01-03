using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCore.Extension;
using NetCore.Jwt;
using NetCore.Security;
using NetCore.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetCore.WebApi
{
    public static class WebApiExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            return services.AddWebApi("Infrastructure:WebApi");
        }

        public static IServiceCollection AddWebApi(this IServiceCollection services, string configureSection)
        {
            if (string.IsNullOrWhiteSpace(configureSection))
            {
                throw new ArgumentException("configureSection");
            }
            IConfiguration val = services.FindConfiguration();
            WebApiOptions webApiOptions = new WebApiOptions();
            ConfigurationBinder.Bind(val, configureSection, (object)webApiOptions);
            OptionsConfigurationServiceCollectionExtensions.Configure<WebApiOptions>(services, val.GetSection(configureSection));
            return AddWebApiCore(services, webApiOptions);
        }

        public static IServiceCollection AddWebApi(this IServiceCollection services, Action<WebApiOptions> configureAction)
        {
            if (configureAction == null)
            {
                throw new ArgumentNullException("configureAction");
            }
            WebApiOptions webApiOptions = new WebApiOptions();
            configureAction(webApiOptions);
            OptionsServiceCollectionExtensions.Configure<WebApiOptions>(services, configureAction);
            return AddWebApiCore(services, webApiOptions);
        }

        private static IServiceCollection AddWebApiCore(IServiceCollection services, WebApiOptions options)
        {
            if (!options.DisableJwtAuthentication)
            {
                services.AddJwt();
            }
            if (!options.DisableHeaderSession)
            {
                services.AddApiSession();
            }
            MvcJsonMvcBuilderExtensions.AddJsonOptions(MvcServiceCollectionExtensions.AddMvc(services, (Action<MvcOptions>)delegate (MvcOptions p)
            {
                if (!options.DisableJwtAuthentication)
                {
                    ((Collection<IFilterMetadata>)p.Filters).Add(new JwtAuthorizeFilter());
                }
                p.Filters.Add<ModelStateValidateAttribute>();
                p.Filters.Add<ActionArgumentsSnapshotAttribute>();
            }), (Action<MvcJsonOptions>)delegate (MvcJsonOptions p)
            {
                SecurityContractResolver securityContractResolver = new SecurityContractResolver();
                securityContractResolver.NamingStrategy = new CamelCaseNamingStrategy();
                SecurityContractResolver contractResolver = securityContractResolver;
                p.SerializerSettings.ContractResolver = contractResolver;
                JsonConvert.DefaultSettings = () => p.SerializerSettings;
            });
            if (!options.DisableCORS)
            {
                CorsServiceCollectionExtensions.AddCors(services, c =>
                {
                    HashSet<string> origins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (options.CORS.Origins != null)
                    {
                        options.CORS.Origins.ForEach(delegate (string r)
                        {
                            origins.Add(r);
                            origins.Add("http://" + r);
                            origins.Add("https://" + r);
                        });
                    }
                    List<string> exposedHeaders = options.CORS.ExposedHeaders ?? new List<string>();
                    c.AddPolicy("cors", policy =>
                   {
                       policy.WithOrigins(origins.ToArray()).WithExposedHeaders(exposedHeaders.ToArray()).WithMethods(new string[4]
                       {
                        HttpMethods.Get,
                        HttpMethods.Post,
                        HttpMethods.Put,
                        HttpMethods.Delete
                       })
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .SetPreflightMaxAge(TimeSpan.FromHours(1.0));
                   });
                });
            }
            return services;
        }

        public static IApplicationBuilder UseWebApi(this IApplicationBuilder app)
        {
            IOptions<WebApiOptions> service = ServiceProviderServiceExtensions.GetService<IOptions<WebApiOptions>>(app.ApplicationServices);
            if (!service.Value.DisableCORS)
            {
                CorsMiddlewareExtensions.UseCors(app, "cors");
            }
            if (!service.Value.DisableJwtAuthentication)
            {
                UseWhenExtensions.UseWhen(app, (Func<HttpContext, bool>)((HttpContext context) => HttpMethods.IsGet(context.Request.Method)), (Action<IApplicationBuilder>)delegate (IApplicationBuilder app2)
                {
                    UseMiddlewareExtensions.UseMiddleware<QueryStringAuthTokenMiddleware>(app2, Array.Empty<object>());
                });
                AuthAppBuilderExtensions.UseAuthentication(app);
                app.UseJwtAuthorize();
            }
            if (!service.Value.DisableHeaderSession)
            {
                app.UseHeaderSession();
            }
            MvcApplicationBuilderExtensions.UseMvc(app, (Action<IRouteBuilder>)delegate (IRouteBuilder route)
            {
                MapRouteRouteBuilderExtensions.MapRoute(route, "api", "{controller}/{action}");
            });
            return app;
        }
    }
}
