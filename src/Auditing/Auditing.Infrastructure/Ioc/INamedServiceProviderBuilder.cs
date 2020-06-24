using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public interface INamedServiceProviderBuilder
    {
        INamedServiceProviderBuilder AddNamedService<TService>(string serviceName, ServiceLifetime lifetime) where TService : class;

        INamedServiceProviderBuilder TryAddNamedService<TService>(string serviceName, ServiceLifetime lifetime) where TService : class;
        void Build();
    }
}
