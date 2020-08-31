using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.EventBus
{
    public class ChannelObjectPoolPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly ConnectionFactory _connectionFactory;
        public ChannelObjectPoolPolicy(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IModel Create()
        {
            var connection = _connectionFactory.CreateConnection();
            return connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (!obj.IsOpen)
            {
                obj?.Dispose();
                return false;
            }

            return true;
        }
    }
}
