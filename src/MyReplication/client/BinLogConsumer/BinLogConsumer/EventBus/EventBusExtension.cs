using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BinLogConsumer.EventBus
{
    public static class EventBusExtension
    {
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var eventBus = new RabbitMQEventBus(sp.GetRequiredService<IRabbitMQPersistentConnection>(), sp.GetRequiredService<IEventBusSubscriptionManager>(), sp.GetRequiredService<ILogger<RabbitMQEventBus>>(), sp, "eventbus-exchange", "eventbus-queue");
                eventBus.SubscribeAll();
                return eventBus;
            });

            var eventHandlers = GetEventHandlers();
            if (eventHandlers != null && eventHandlers.Any())
                eventHandlers.ToList().ForEach(x => services.AddTransient(x));
        }

        private static IEnumerable<Type> FromThis()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var feferdAssemblies = entryAssembly.GetReferencedAssemblies().Select(x => Assembly.Load(x));
            var allAssemblies = new List<Assembly> { entryAssembly }.Concat(feferdAssemblies);
            return allAssemblies.SelectMany(x => x.DefinedTypes).ToList();
        }


        private static void SubscribeAll(this IEventBus eventBus)
        {
            var types = FromThis();

            var allEventTypes = types
                .Where(x => typeof(EventBase).IsAssignableFrom(x) && x != typeof(EventBase))
                .ToList();

            var allEventHandlerTypes = types
                .Where(x => typeof(IEventHandlerBase).IsAssignableFrom(x) && x != typeof(IEventHandlerBase) && x != typeof(IEventHandler<>))
                .ToList();

            foreach(var eventType in allEventTypes)
            {
                foreach (var eventHandlerType in allEventHandlerTypes.FindAll(x => typeof(IEventHandler<>).MakeGenericType(x).IsAssignableFrom(eventType)))
                {
                    typeof(IEventBus).GetMethod("Subscribe").MakeGenericMethod(eventType, eventHandlerType).Invoke(eventBus, null);
                }
            }
        }

        private static IEnumerable<Type> GetEventHandlers()
        {
            return FromThis().Where(x => typeof(IEventHandlerBase).IsAssignableFrom(x) && x != typeof(IEventHandlerBase) && x != typeof(IEventHandler<>));
        }
    }
}
