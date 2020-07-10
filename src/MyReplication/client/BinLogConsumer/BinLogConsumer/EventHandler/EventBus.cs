using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BinLogConsumer.EventHandler
{
    public class EventBus : IEventBus
    {
        private readonly ILogger<EventBus> _eventBusLogger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly Dictionary<string, List<Type>> _eventHandlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceProvider _serviceProvider;
        private AsyncEventingBasicConsumer _eventConsumer;

        public EventBus(IConnectionFactory connectionFactory, ILogger<EventBus> eventBusLogger, IServiceProvider serviceProvider)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(IConnectionFactory));
            _eventBusLogger = eventBusLogger ?? throw new ArgumentNullException(nameof(ILogger<EventBus>));
            _eventHandlers = new Dictionary<string, List<Type>>();
            _serviceProvider = serviceProvider;
            _eventTypes = new List<Type>();
        }

        public void Publish<TEvent>(TEvent @event) where TEvent:EventBase
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("binlogExchange", "direct");

                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);
                    var eventName = @event.GetType().Name;

                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    channel.BasicPublish(exchange: "binlogExchange", routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
                }
            }
        }


        public void Subscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers.Add(eventName, new List<Type> { typeof(TH) });
                _eventTypes.Add(typeof(T));
            }
            else if (_eventHandlers.ContainsKey(eventName) && !_eventHandlers[eventName].Exists(x => x == typeof(TH)))
            {
                _eventHandlers[eventName].Add(typeof(TH));
            }

            StartConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].RemoveAll(x => x == typeof(TH));
            }
        }

        private void StartConsume()
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("binlogExchange", "direct");
            channel.QueueDeclare("binlogQueue", true, false, false, null);
            channel.CallbackException += (sender, args) => StartConsume();
            _eventConsumer = new AsyncEventingBasicConsumer(channel);
            _eventConsumer.Received += async (sender, args) =>
            {
                var eventName = args.RoutingKey;
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                try
                {
                    if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                        throw new InvalidOperationException($"Fake exception requested: \"{message}\"");

                    await ProcessEvent(eventName, message);
                }
                catch (Exception ex)
                {
                    _eventBusLogger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
                }

                channel.BasicAck(args.DeliveryTag, multiple: false);
            };
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    foreach(var handlerType in _eventHandlers[eventName])
                    {
                        var handler = serviceScope.ServiceProvider.GetRequiredService(handlerType);
                        if (handler == null) continue;

                        var eventType = _eventTypes.FirstOrDefault(x => x.Name == eventName); ;
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await(Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
    }
}
