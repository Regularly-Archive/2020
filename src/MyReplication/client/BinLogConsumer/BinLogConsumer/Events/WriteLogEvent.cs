using BinLogConsumer.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Events
{
    [Serializable]
    public class WriteLogEvent : EventBase
    {
        public string TRANSACTION_ID { get; set; }
        public string LOG_LEVEL { get; set; }
        public string HOST_NAME { get; set; }
        public string HOST_IP { get; set; }
        public string CONTENT { get; set; }
        public string USER_ID { get; set; }
        public string TTID { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string APP_NAMESPACE { get; set; }
    }
}
