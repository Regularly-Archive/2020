using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BinLogConsumer.EventBus;
using BinLogConsumer.Events;
using BinLogConsumer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Route("PublishBinLog")]
        public Task PublishBinLog(BinLogEventModel<dynamic> eventModel)
        {
            if (eventModel.action == "insert" && eventModel.table.StartsWith("log_"))
                _eventBus.Publish(eventModel.MapTo<WriteLogEvent>());

            if (eventModel.action == "insert" && eventModel.table == "order_info")
                _eventBus.Publish(eventModel.MapTo<OrderInfoCreateEvent>());

            return Task.CompletedTask;
        }
    }
}
