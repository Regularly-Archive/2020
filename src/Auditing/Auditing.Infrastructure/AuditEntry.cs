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

        public void SetValuesCollection(EntityEntry entityEntry, List<PropertyEntry> properties)
        {
            foreach(var property in properties)
            {
                var propertyName = property.Metadata.GetColumnName();
                if (_auditConfig.PropertyFilters.Any(x => x(entityEntry, property)))
                    continue;

                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        NewValues.AddOrUpdate(propertyName, property.CurrentValue);
                        break;
                    case EntityState.Modified:
                        if (!_auditConfig.IsIgnoreSameValue || property.OriginalValue.ToString() == property.CurrentValue.ToString())
                        OldValues.AddOrUpdate(propertyName, property.OriginalValue);
                        NewValues.AddOrUpdate(propertyName, property.CurrentValue);
                        break;
                    case EntityState.Deleted:
                        NewValues.AddOrUpdate(propertyName, property.OriginalValue);
                        break;
                }
            };
        }

        private void GetTemporaryProperties(EntityEntry entityEntry)
        {
            TemporaryProperties = entityEntry.Properties.ToList().FindAll(property => property.IsTemporary).ToList();
        }
    }

    public static class DictionaryExtension
    {
        public static void AddOrUpdate<TKey,TValue>(this Dictionary<TKey,TValue> dict, TKey key, TValue value)
        {
            var action = dict.ContainsKey(key) ? new Action(() => dict[key] = value) : new Action(() => dict.Add(key, value));
            action();
        }
    }
}
