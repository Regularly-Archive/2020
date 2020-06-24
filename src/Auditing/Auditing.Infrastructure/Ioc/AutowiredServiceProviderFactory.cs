using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class AutowiredServiceProviderFactory :IServiceProviderFactory<IServiceCollection>
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider();
            return new AutowiredServiceProvider(serviceProvider);
        }

        IServiceCollection IServiceProviderFactory<IServiceCollection>.CreateBuilder(IServiceCollection services)
        {
            if (services == null) return new ServiceCollection();
            return services;
        }
    }
}
