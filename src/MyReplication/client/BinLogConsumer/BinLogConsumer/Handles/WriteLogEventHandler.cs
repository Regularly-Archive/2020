using BinLogConsumer.EventHandler;
using BinLogConsumer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Handles
{
    public class WriteLogEventHandler : IEventHandler<WriteLogEvent>
    {
        private ILogger<WriteLogEventHandler> _logger;

        public WriteLogEventHandler(ILogger<WriteLogEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(WriteLogEvent @event)
        {
            _logger.LogInformation($"当前消费消息:{@event.TRANSACTION_ID}-{@event.LOG_LEVEL}-{@event.HOST_NAME}-${@event.HOST_IP}-${@event.CONTENT}");
            return Task.CompletedTask;
        }
    }
}
