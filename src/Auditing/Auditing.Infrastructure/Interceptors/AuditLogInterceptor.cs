using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auditing.Domain;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Auditing.Infrastructure.Interceptors
{
    public class AuditLogInterceptor : IInterceptor,IAuditStorage
    {
        public void Intercept(IInvocation invocation)
        {
            //检测是否需要写审计日志
            var methodInfo = invocation.Method;
            var auditLogAttrs = methodInfo.GetCustomAttributes(typeof(AuditLogAttribute), false);
            if (auditLogAttrs == null || auditLogAttrs.Length == 0 || invocation.Arguments[0].GetType() == typeof(AuditLog))
            {
                invocation.Proceed();
                return;
            }

            //写入审计日志
            var auditLogAttr = (auditLogAttrs as AuditLogAttribute[])[0];
            var auditLogs = new List<AuditLog>();
            switch (auditLogAttr.OperationType)
            {
                case Domain.OperationType.Created:
                    auditLogs = invocation.Arguments.ToList().Select(x => new AuditLog()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        NewValues = JsonConvert.SerializeObject(x),
                        OldValues = null,
                        ExtraData = null,
                        CreatedBy = string.Empty,
                        CreatedDate = DateTime.Now,
                        OperationType = (int)auditLogAttr.OperationType
                    })
                    .ToList();
                    break;
                case Domain.OperationType.Updated:
                    break;
                case Domain.OperationType.Deleted:
                    auditLogs = invocation.Arguments.ToList().Select(x => new AuditLog()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        NewValues = null,
                        OldValues = JsonConvert.SerializeObject(x),
                        ExtraData = null,
                        CreatedBy = string.Empty,
                        CreatedDate = DateTime.Now,
                        OperationType = (int)auditLogAttr.OperationType
                    })
                    .ToList();
                    break;
            }
            


            //执行正常逻辑
            invocation.Proceed();
        }

        public void SaveAuditLogs(params AuditLog[] auditLogs)
        {
            
        }
    }
}
