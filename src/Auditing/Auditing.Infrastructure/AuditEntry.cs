using System;
using System.Collections.Generic;
using System.Linq;
using Auditing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Auditing.Infrastructure
{
    public class AuditEntry
    {
        private readonly AuditConfig _auditConfig;
        private string TableName { get; set; }
        public OperationType OperationType { get; set; }
        public Dictionary<string, object> OldValues { get; private set; }
        public Dictionary<string, object> NewValues { get; private set; }
        public Dictionary<string, object> ExtraData { get; private set; }
        public List<PropertyEntry> TemporaryProperties { get; set; } = new List<PropertyEntry>();

        public AuditEntry(EntityEntry entityEntry, AuditConfig auditConfig)
        {
            _auditConfig = auditConfig;
            TableName = entityEntry.Metadata.GetTableName();
            OldValues = new Dictionary<string, object>();
            NewValues = new Dictionary<string, object>();
            ExtraData = new Dictionary<string, object>();
            OperationType = SetOperationType(entityEntry);
            SetValuesCollection(entityEntry);
            SetTemporaryProperties(entityEntry);
        }

        public AuditLog AsAuditLog()
        {
            return new AuditLog()
            {
                Id = Guid.NewGuid().ToString("N"),
                TableName = TableName,
                CreatedBy = string.Empty,
                CreatedDate = DateTime.Now,
                NewValues = NewValues.Any() ? JsonConvert.SerializeObject(NewValues) : null,
                OldValues = OldValues.Any() ? JsonConvert.SerializeObject(OldValues) : null,
                ExtraData = ExtraData.Any() ? JsonConvert.SerializeObject(ExtraData) : null,
                OperationType = (int)OperationType
            };
        }

        private OperationType SetOperationType(EntityEntry entityEntry)
        {
            var opType = OperationType.Created;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    opType = OperationType.Created;
                    break;
                case EntityState.Modified:
                    opType = OperationType.Updated;
                    break;
                case EntityState.Deleted:
                    opType = OperationType.Deleted;
                    break;
            }

            return opType;
        }

        private void SetValuesCollection(EntityEntry entityEntry)
        {
            var properties = entityEntry.Properties.ToList().FindAll(property => !property.IsTemporary).ToList();
            foreach(var property in properties)
            {
                if (_auditConfig.PropertyFilters.Any(x => x(entityEntry, property)))
                    continue;

                //确保字典内Key存在
                var fieldName = property.Metadata.GetColumnName();
                NewValues.Add(fieldName, null);
                OldValues.Add(fieldName, null);

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        NewValues[fieldName] = property.CurrentValue;
                        break;
                    case EntityState.Modified:
                        OldValues[fieldName] = property.OriginalValue;
                        NewValues[fieldName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        OldValues[fieldName] = property.OriginalValue;
                        break;
                }
            };
        }

        private List<PropertyEntry> SetTemporaryProperties(EntityEntry entityEntry)
        {
            return entityEntry.Properties.ToList().FindAll(property => property.IsTemporary).ToList();
        }
    }
}
