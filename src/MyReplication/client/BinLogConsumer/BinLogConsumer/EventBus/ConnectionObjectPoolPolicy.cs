using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.EventBus
{
    public class ConnectionObjectPoolPolicy : IPooledObjectPolicy<IConnection>
    {
        private readonly ConnectionFactory _connectionFactory;
        public ConnectionObjectPoolPolicy(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool Return(IConnection obj)
        {
            if (!obj.IsOpen)
            {
                obj?.Dispose();
                return false;
            }

            return true;
        }

        IConnection IPooledObjectPolicy<IConnection>.Create()
        {
            return _connectionFactory.CreateConnection();
        }
    }
}
