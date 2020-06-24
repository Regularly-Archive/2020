using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class AutowiredServiceProviderFactory :IServiceProviderFactory<ServiceCollection>
    {
        public ServiceCollection CreateBuilder(IServiceCollection services)
        {
            if (services == null) return new ServiceCollection();
            return (ServiceCollection)services;
        }

        public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider();
            return new AutowiredServiceProvider(serviceProvider);
        }
    }
}
