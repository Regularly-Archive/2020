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
        private string TableName { get; set; }
        private readonly EntityEntry _entityEntry;
        private readonly AuditConfig _auditConfig;
        public OperationType OperationType { get; set; }
        public Dictionary<string, object> OldValues { get; private set; }
        public Dictionary<string, object> NewValues { get; private set; }
        public Dictionary<string, object> ExtraData { get; private set; }
        private List<PropertyEntry> TemporaryProperties { get; set; } = new List<PropertyEntry>();

        public AuditEntry(EntityEntry entityEntry, AuditConfig auditConfig)
        {
            _auditConfig = auditConfig;
            _entityEntry = entityEntry;
            TableName = entityEntry.Metadata.GetTableName();
            OldValues = new Dictionary<string, object>();
            NewValues = new Dictionary<string, object>();
            ExtraData = new Dictionary<string, object>();
            OperationType = GetOperationType();
            SetValuesCollection(_entityEntry.Properties.ToList().FindAll(property => !property.IsTemporary).ToList());
            InitTemporaryProperties();
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

        public void UpdateTemporaryProperties()
        {
            if (TemporaryProperties == null || !TemporaryProperties.Any())
                return;

            SetValuesCollection(TemporaryProperties);
            TemporaryProperties = null;
        }

        private OperationType GetOperationType()
        {
            var opType = OperationType.Created;
            switch (_entityEntry.State)
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

        private void SetValuesCollection(List<PropertyEntry> properties)
        {
            foreach (var property in properties)
            {
                var propertyName = property.Metadata.GetColumnName();
                if (_auditConfig.PropertyFilters.Any(x => x(_entityEntry, property)))
                    continue;

                switch (OperationType)
                {
                    case OperationType.Created:
                        NewValues[propertyName] = property.CurrentValue;
                        break;
                    case OperationType.Updated:
                        if (_auditConfig.IsIgnoreSameValue && property.OriginalValue.ToString() == property.CurrentValue.ToString())
                            continue;
                        OldValues[propertyName] = property.OriginalValue;
                        NewValues[propertyName] = property.CurrentValue;
                        break;
                    case OperationType.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        break;
                }
            };
        }

        private void InitTemporaryProperties()
        {
            TemporaryProperties = _entityEntry.Properties.ToList().FindAll(property => property.IsTemporary).ToList();
        }
    }

    public static class DictionaryExtension
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            (dict.ContainsKey(key) ? new Action(() => dict[key] = value) : new Action(() => dict.Add(key, value)))();
        }
    }
}
