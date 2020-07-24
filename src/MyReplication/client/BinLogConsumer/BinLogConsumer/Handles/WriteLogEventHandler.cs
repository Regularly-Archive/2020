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
            _logger.LogInformation($"日志编号：{@event.TRANSACTION_ID}，日志级别：{@event.LOG_LEVEL}，主机：{@event.HOST_NAME}，IP：{@event.HOST_IP}，内容：{@event.CONTENT}");
            return Task.CompletedTask;
        }
    }
}
