using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
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
        private readonly int _retryCount; //消息重试次数

        public RabbitMQEventBus(IRabbitMQPersistentConnection persistentConnection, IEventBusSubscriptionManager subscriptionManager, ILogger<RabbitMQEventBus> logger, IServiceProvider serviceProvider, string exchangeName, string queueName, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(IConnectionFactory));
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(IEventBusSubscriptionManager));
            _subscriptionManager.OnSubscribe += (s, e) => StartBasicConsume(e.EvenType.FullName);
            _subscriptionManager.OnUnsubscribe += (s, e) => UnbindQueue(e.EvenType.FullName);
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger<RabbitMQEventBus>));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
            _exchangeName = exchangeName ?? "rabbitmq-eventbus";
            _retryCount = retryCount;
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(_exchangeName, "direct", true, false, null);

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
        }

        public void Unsubscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            _subscriptionManager.Unsubscribe<T, TH>();
        }

        private void StartBasicConsume(string routingKey)
        {
            _logger.LogTrace("Starting RabbitMQ BasicConsume...");

            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

            var queueName = GetQueueName(routingKey);
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(_exchangeName, "direct", true, false, null);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, _exchangeName, routingKey, null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (s, e) =>
            {
                var routingKey = e.RoutingKey;
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                var tasks = ProcessEvent(routingKey, message);
                await Task.WhenAll(tasks);
                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queue: $"Q:{routingKey}", autoAck: false, consumer: consumer);
        }

        private void UnbindQueue(string routingKey)
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();
            var channel = _persistentConnection.CreateModel();
            var queueName = GetQueueName(routingKey);
            channel.QueueUnbind(queueName, _exchangeName, routingKey, null);
        }

        private string GetQueueName(string routingKey)
        {
            return $"Q:{routingKey}";
        }

        private IEnumerable<Task> ProcessEvent(string eventName, string message)
        {
            if (_subscriptionManager.IsEventSubscribed(eventName))
            {
                var policy = BuildProcessEventPolicy();
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    foreach (var handlerType in _subscriptionManager.GetHandlersForEvent(eventName))
                    {
                        var handler = serviceScope.ServiceProvider.GetRequiredService(handlerType);
                        if (handler == null) continue;

                        var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        _logger.LogInformation($"Process event \"{eventName}\" with \"{handler.GetType().Name}\"...");
                        yield return (Task)policy.Execute(() => concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent }));
                    }
                }
            }
        }

        private  PolicyWrap BuildProcessEventPolicy()
        {
            //重试策略
            var retryPolicy = RetryPolicy
                .Handle<Exception>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogInformation($"Process event fails due to \"{ex.InnerException?.Message}\" and  it will re-try after {time.TotalSeconds}s");
                });

            //超时策略
            var timeoutPolicy = TimeoutPolicy.Timeout(30);

            //组合策略
            return retryPolicy.Wrap(timeoutPolicy);
        }
    }
}
