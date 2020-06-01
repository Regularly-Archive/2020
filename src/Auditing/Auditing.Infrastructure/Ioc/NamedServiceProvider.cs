using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    class NamedServiceProvider : INamedServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _registrations;
        public NamedServiceProvider(IServiceProvider serviceProvider, IDictionary<string, Type> registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
        }

        public TService GetService<TService>(string serviceName)
        {
            if(!_registrations.TryGetValue(serviceName, out var implementationType))
                throw new ArgumentException($"Service \"{serviceName}\" is not registered in container");
            return (TService)_serviceProvider.GetService(implementationType);
        }
    }
}
