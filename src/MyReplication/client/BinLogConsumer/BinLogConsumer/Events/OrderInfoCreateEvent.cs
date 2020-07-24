using BinLogConsumer.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Events
{
    [Serializable]
    public class OrderInfoCreateEvent : EventBase
    {
        public string ORDER_ID { get; set; }
    }
}
