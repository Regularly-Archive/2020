using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    public static class AuditExtension
    {
        public static void AddAuditLog(this IServiceCollection servicesCollection, Action<AuditConfig> configure)
        {
            var config = AuditConfig.Default;
            configure(config);
            servicesCollection.AddSingleton(typeof(AuditConfig), config);
        }
    }
}
