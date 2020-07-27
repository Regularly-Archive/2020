using BinLogConsumer.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer
{
    public class EventBusSubscriptionEventArgs
    {
        public Type EvenType { get; set; }
        public Type HandlerType { get; set; }
    }
}
