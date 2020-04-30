using Auditing.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false)]
    public class AuditLogAttribute:Attribute
    {
        public OperationType OperationType { get; set; }
        public AuditLogAttribute(OperationType operationType)
        {
            OperationType = operationType;
        }
    }
}
