using Auditing.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    public class FileAuditStorage : IAuditStorage
    {
        public void SaveAuditLogs(params AuditLog[] auditLogs)
        {
            throw new NotImplementedException();
        }
    }
}
