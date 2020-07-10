using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinLogConsumer.EventHandler;
using BinLogConsumer.Events;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BinLogConsumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventBusController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        public EventBusController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // Post: /<controller>/Publish
        [HttpPost]
        public Task Publish(WriteLogEvent @event)
        {
            _eventBus.Publish(@event);
            return Task.CompletedTask;
        }
    }
}
