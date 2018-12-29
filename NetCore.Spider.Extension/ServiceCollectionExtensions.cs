using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace NetCore.Spider.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IConfiguration FindConfiguration(this IServiceCollection services)
        {
            ServiceDescriptor val = ((IEnumerable<ServiceDescriptor>)services).FirstOrDefault((ServiceDescriptor s) => s.ServiceType == typeof(IConfiguration));
            if (val == null)
            {
                return null;
            }
            return val.ImplementationInstance as IConfiguration;
        }
    }
}
