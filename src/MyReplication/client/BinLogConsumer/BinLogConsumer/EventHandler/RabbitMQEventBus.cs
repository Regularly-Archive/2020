using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BinLogConsumer.EventHandler
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly string _exchangeName; //交换器名称
        private readonly string _queueName; //队列名称
        private IModel _consumeChannel; //消费者Channel
        private readonly int _retryCount; //消息重试次数

        public RabbitMQEventBus(IRabbitMQPersistentConnection persistentConnection, IEventBusSubscriptionManager subscriptionManager, ILogger<RabbitMQEventBus> logger, IServiceProvider serviceProvider, string exchangeName, string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(IConnectionFactory));
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(IEventBusSubscriptionManager));
            _subscriptionManager.OnSubscribe += (s, e) => StartBasicConsume();
            _subscriptionManager.OnUnsubscribe += (s, e) => { };
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<RabbitMQEventBus>));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
            _exchangeName = exchangeName ?? "rabbitmq-eventbus";
            _queueName = queueName ?? "eventbus-queue"; ;
            _consumeChannel = CreateConsumerChannel();
            _retryCount = retryCount;
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(_exchangeName, "direct");
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(_queueName, _exchangeName, "", null);

                var eventName = @event.GetType().FullName;
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                channel.BasicPublish(exchange: _exchangeName, routingKey: eventName, mandatory: true, basicProperties: null, body: body);
                _logger.LogDebug($"Publish message with RabbmitMQ BasicPublish: {message}");
            }
        }


        public void Subscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            _subscriptionManager.Subscribe<T, TH>();
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            _subscriptionManager.Unsubscribe<T, TH>();
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ BasicConsume...");
            if (_consumeChannel == null)
            {
                _logger.LogError("StartBasicConsume can't be called on NullReference of _consumerChannel");
                return;
            }

            var consumer = new EventingBasicConsumer(_consumeChannel);
            consumer.Received += (s, e) =>
            {
                var eventName = e.RoutingKey;
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var policy = RetryPolicy.Handle<Exception>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, "RabbitMQ Client consume messafe after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

                policy.Execute(async () => await ProcessEvent(eventName, message));
            };
            _consumeChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }

        private IModel CreateConsumerChannel()
        {
            _logger.LogTrace("Creating RabbitMQ Consune Channel...");
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
            var channel = _persistentConnection.CreateModel();
            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subscriptionManager.IsEventSubscribed(eventName))
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    foreach (var handlerType in _subscriptionManager.GetHandlersForEvent(eventName))
                    {
                        var handler = serviceScope.ServiceProvider.GetRequiredService(handlerType);
                        if (handler == null) continue;

                        var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        _logger.LogDebug($"Process event \"{eventName}\" via handler \"{handler.GetType().Name}\"");
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
    }
}
