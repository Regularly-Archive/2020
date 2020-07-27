using BinLogConsumer.EventBus;
using BinLogConsumer.Events;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Handles
{
    public class AnalyseLogEventHandler : IEventHandler<WriteLogEvent>
    {
        private readonly ILogger<AnalyseLogEventHandler> _logger;
        private readonly IDistributedCache _cache;

        public AnalyseLogEventHandler(ILogger<AnalyseLogEventHandler> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public Task Handle(WriteLogEvent @event)
        {
            var cacheCount = _cache.GetString(@event.LOG_LEVEL);
            if (string.IsNullOrEmpty(cacheCount))
                cacheCount = "1";
            else
                cacheCount = (int.Parse(cacheCount) + 1).ToString();

            _cache.SetString(@event.LOG_LEVEL, cacheCount); ;
            return Task.CompletedTask;
        }
    }
}
