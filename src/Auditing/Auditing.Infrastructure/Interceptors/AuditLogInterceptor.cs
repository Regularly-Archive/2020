using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Auditing.Domain;
using Auditing.Infrastructure.Repository;
using Castle.DynamicProxy;
using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Auditing.Infrastructure.Interceptors
{
    public class AuditLogInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var repository = invocation.Proxy as IRepository;
            var auditLogAttr = invocation.Method.GetCustomAttribute<AuditLogAttribute>();
            if (auditLogAttr == null)
            {
                invocation.Proceed();
                return;
            }

            var entityType = GetEntityType(invocation);
            var tableName = GetTableName(entityType);
            var tableIdProperty = entityType.GetProperty("Id");


            var auditLogs = new List<AuditLog>();
            switch (auditLogAttr.OperationType)
            {
                case Domain.OperationType.Created:
                    auditLogs = GetAddedAuditLogs(invocation, tableName);
                    break;
                case Domain.OperationType.Updated:
                    auditLogs = GetUpdatedAuditLogs(invocation, tableName, entityType, tableIdProperty, repository);
                    break;
                case Domain.OperationType.Deleted:
                    auditLogs = GetDeletedAuditLogs(invocation, tableName);
                    break;
            }
            
            invocation.Proceed();
            repository.Insert<AuditLog>(auditLogs.ToArray());
        }

        private Type GetEntityType(IInvocation invocation)
        {
            return (invocation.Arguments[0] as object[])[0].GetType();
        }

        private string GetTableName(Type entityType)
        {
            var tableName = entityType.Name;
            var tabLeNameAttrs = entityType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tabLeNameAttrs != null && tabLeNameAttrs.Length > 0)
                tableName = (tabLeNameAttrs[0] as TableAttribute).Name;
            return tableName;
        }

        private bool IsAuditLogEnable(IInvocation invocation, Type entityType)
        {
            var methodInfo = invocation.Method;
            var auditLogAttrs = methodInfo.GetCustomAttributes(typeof(AuditLogAttribute), false);
            if (auditLogAttrs == null || auditLogAttrs.Length == 0 || entityType == typeof(AuditLog))
                return false;

            return true;
        }

        private List<AuditLog> GetAddedAuditLogs(IInvocation invocation, string tableName)
        {
            var auditLogs = (invocation.Arguments[0] as object[]).Select(x => new AuditLog()
            {
                Id = Guid.NewGuid().ToString("N"),
                NewValues = JsonConvert.SerializeObject(x),
                OldValues = null,
                ExtraData = null,
                CreatedBy = string.Empty,
                CreatedDate = DateTime.Now,
                OperationType = (int)OperationType.Created,
                TableName = tableName
            })
            .ToList();

            return auditLogs;
        }

        private List<AuditLog> GetDeletedAuditLogs(IInvocation invocation, string tableName)
        {
            var auditLogs = (invocation.Arguments[0] as object[]).Select(x => new AuditLog()
            {
                Id = Guid.NewGuid().ToString("N"),
                NewValues = null,
                OldValues = JsonConvert.SerializeObject(x),
                ExtraData = null,
                CreatedBy = string.Empty,
                CreatedDate = DateTime.Now,
                OperationType = (int)OperationType.Deleted,
                TableName = tableName
            })
            .ToList();

            return auditLogs;
        }

        private List<AuditLog> GetUpdatedAuditLogs(IInvocation invocation, string tableName, Type entityType, PropertyInfo tableIdProperty, IRepository repository)
        {
            var auditLogs = new List<AuditLog>();
            var entitiesKeys = (invocation.Arguments[0] as object[]).Select(x => tableIdProperty.GetValue(x, null)).ToList();
            var originEntities = typeof(IRepository).GetMethod("GetByQuery").MakeGenericMethod(entityType).Invoke(repository, new object[]
            {
                $"SELECT * FROM {tableName} WHERE Id IN @Ids", entitiesKeys
            });


            foreach (var newEntity in (invocation.Arguments[0] as object[]))
            {
                var entityKey = tableIdProperty.GetValue(newEntity, null);
                auditLogs.Add(new AuditLog()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    OldValues = null,
                    NewValues = JsonConvert.SerializeObject(newEntity),
                    ExtraData = null,
                    CreatedBy = string.Empty,
                    CreatedDate = DateTime.Now,
                    OperationType = (int)OperationType.Updated,
                    TableName = tableName
                });
            }

            return auditLogs;
        }
    }
}
