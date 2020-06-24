using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Autofac.Core;
using Autofac;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auditing.Infrastructure.Ioc
{
    public static class ServiceCollectionExstension
    {
        public static TService GetNamedService<TService>(this IServiceProvider serviceProvider, string serviceName)
        {
            var namedServiceProvider = serviceProvider.GetRequiredService<INamedServiceProvider>();
            if (namedServiceProvider == null)
                throw new ArgumentException($"Service \"{nameof(INamedServiceProvider)}\" is not registered in container");

            return namedServiceProvider.GetService<TService>(serviceName);
        }


        public static INamedServiceProviderBuilder AsNamedServiceProvider(this IServiceCollection services)
        {
            var builder = new NamedServiceProviderBuilder(services);
            return builder;
        }
    }
}
