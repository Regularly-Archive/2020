using RabbitMQ.Client;
using System;

namespace BinLogConsumer.EventBus
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();

        void ReturnModel(IModel obj);
    }
}
