using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public interface INamedServiceProviderBuilder
    {
        INamedServiceProviderBuilder AddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        INamedServiceProviderBuilder TryAddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        INamedServiceProviderBuilder AddNamedService<TService>(string serviceName, ServiceLifetime lifetime) where TService : class;

        void Build();
    }
}
