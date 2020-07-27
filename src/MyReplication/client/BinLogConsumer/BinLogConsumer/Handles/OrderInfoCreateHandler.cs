using BinLogConsumer.EventBus;
using BinLogConsumer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Handles
{
    public class OrderInfoCreateHandler : IEventHandler<OrderInfoCreateEvent>
    {
        private ILogger<OrderInfoCreateEvent> _logger;

        public OrderInfoCreateHandler(ILogger<OrderInfoCreateEvent> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderInfoCreateEvent @event)
        {
            _logger.LogInformation($"发货订单{@event.ORDER_ID}已创建");
            return Task.CompletedTask;
        }
    }
}
