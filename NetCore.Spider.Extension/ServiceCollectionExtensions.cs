using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace NetCore.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IConfiguration FindConfiguration(this IServiceCollection services)
        {
            ServiceDescriptor serviceDescriptor = ((IEnumerable<ServiceDescriptor>)services).FirstOrDefault((ServiceDescriptor s) => s.ServiceType == typeof(IConfiguration));
            if (serviceDescriptor == null)
            {
                return null;
            }
            return serviceDescriptor.ImplementationInstance as IConfiguration;
        }
    }
}
