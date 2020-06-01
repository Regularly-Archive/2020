using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class AutowriedServiceProviderFactory :IServiceProviderFactory<IServiceCollection>
    {
        private IServiceProviderFactory<IServiceCollection> _serviceProviderFactory;
        public AutowriedServiceProviderFactory()
        {
            _serviceProviderFactory = new DefaultServiceProviderFactory();
        }

        IServiceCollection IServiceProviderFactory<IServiceCollection>.CreateBuilder(IServiceCollection services)
        {
            return _serviceProviderFactory.CreateBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider();
            return new AutowiredServiceProvider(serviceProvider);
        }
    }
}
