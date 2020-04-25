using Auditing.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    public interface IAuditStorage
    {
        void SaveAuditLogs(params AuditLog[] auditLogs);
    }
}
