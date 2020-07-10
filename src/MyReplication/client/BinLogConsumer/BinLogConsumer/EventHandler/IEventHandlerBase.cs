using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.EventHandler
{
    public interface IEventHandlerBase
    {

    }

    public interface IEventHandler<TEvent> : IEventHandlerBase where TEvent : EventBase
    {
        Task Handle(TEvent @ebent);
    }

    public class EventBase
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
