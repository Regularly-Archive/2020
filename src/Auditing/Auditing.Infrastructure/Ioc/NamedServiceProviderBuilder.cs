using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Auditing.Infrastructure.Ioc
{
    public class NamedServiceProviderBuilder : INamedServiceProviderBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IDictionary<string, Type> _registrations = new Dictionary<string, Type>();
        public NamedServiceProviderBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void Build()
        {
            _services.AddTransient<INamedServiceProvider>(sp => new NamedServiceProvider(sp, _registrations));
        }

        public INamedServiceProviderBuilder AddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime) 
            where TService : class
            where TImplementation : class, TService
        {
            switch(lifetime)
            {
                case ServiceLifetime.Transient:
                    _services.AddTransient<TService, TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    _services.AddScoped<TService, TImplementation>();
                    break;
                case ServiceLifetime.Singleton:
                    _services.AddSingleton<TService, TImplementation>();
                    break;
            }

            _registrations.Add(serviceName, typeof(TImplementation));
            return this;
        }

        public INamedServiceProviderBuilder TryAddNamedService<TService, TImplementation>(string serviceName, ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    _services.TryAddTransient<TService, TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    _services.TryAddScoped<TService, TImplementation>();
                    break;
                case ServiceLifetime.Singleton:
                    _services.TryAddSingleton<TService, TImplementation>();
                    break;
            }

            _registrations.TryAdd(serviceName, typeof(TImplementation));
            return this;
        }

        public INamedServiceProviderBuilder AddNamedService<TImplementation>(string serviceName, ServiceLifetime lifetime) where TImplementation : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    _services.AddTransient<TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    _services.AddScoped<TImplementation>();
                    break;
                case ServiceLifetime.Singleton:
                    _services.AddSingleton<TImplementation>();
                    break;
            }

            _registrations.Add(serviceName, typeof(TImplementation));
            return this;
        }
    }
}
