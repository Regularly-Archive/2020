using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Auditing.Infrastructure
{
    public class AuditConfig
    {
        public bool IsEnableAudit { get; private set; } = true;

        public bool IsIgnoreSameValue { get; private set; } = false;

        public IUserInfoProvider UserInfoProvider { get; set; }

        public List<IAuditStorage> AuditStorages { get; private set; }

        public List<Func<EntityEntry, bool>> EntityFilters { get; private set; }

        public List<Func<EntityEntry, PropertyEntry, bool>> PropertyFilters { get; private set; }

        public List<Action<Dictionary<string, object>>> PropertyEnrichers { get; private set; }

        public AuditConfig()
        {
            AuditStorages = new List<IAuditStorage>();
            EntityFilters = new List<Func<EntityEntry, bool>>();
            PropertyFilters = new List<Func<EntityEntry, PropertyEntry, bool>>();
            PropertyEnrichers = new List<Action<Dictionary<string, object>>>();
        }

        public AuditConfig IgnoreTable(string tableName)
        {
            EntityFilters.Add(entity => entity.Metadata.GetTableName() == tableName);
            return this;
        }

        public AuditConfig IgnoreTable(Type type)
        {
            EntityFilters.Add(entity => entity.GetType() == type);
            return this;
        }

        public AuditConfig IgnoreTable<Entity>()
        {
            EntityFilters.Add(entity => entity.GetType() == typeof(Entity));
            return this;
        }

        public AuditConfig IgnoreTable(Expression<Func<EntityEntry, bool>> lambda)
        {
            EntityFilters.Add(lambda.Compile());
            return this;
        }

        public AuditConfig IgnoreProperty(string propertyName)
        {
            PropertyFilters.Add((entity, property) => property.Metadata.GetColumnName() == propertyName);
            return this;
        }

        public AuditConfig IgnoreProperty(string tableName, string propertyName)
        {
            PropertyFilters.Add((entity, property) => entity.Metadata.GetTableName() == tableName && property.Metadata.GetColumnName() == propertyName);
            return this;
        }

        public AuditConfig IgnoreProperty<Entity>(Expression<Func<Entity, object>> lambda)
        {
            var body = lambda.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)lambda.Body;
                body = ubody.Operand as MemberExpression;
            }

            var propertyName = body.Member.Name;
            PropertyFilters.Add((entity, property) =>
                entity.Metadata.GetTableName() == typeof(Entity).Name && property.Metadata.GetColumnName() == propertyName
            );
            return this;
        }

        public AuditConfig IgnoreProperty(Expression<Func<PropertyEntry, bool>> lambda)
        {
            var func = lambda.Compile();
            PropertyFilters.Add((entity, property) => func(property));
            return this;
        }

        public AuditConfig WithExtraData(string key, object value, bool overwrite = false)
        {
            PropertyEnrichers.Add(obj =>
            {
                if (obj.ContainsKey(key) && overwrite == true)
                {
                    obj[key] = value;
                }
                else
                {
                    obj.Add(key, value);
                }
            });

            return this;
        }

        public AuditConfig WithStorage<TStorage>() where TStorage:IAuditStorage
        {

            return this;
        }

        public AuditConfig WithUserInfoProvider<TProvider>()where TProvider : IUserInfoProvider
        {
            return this;
        }

        public AuditConfig EnableAudit(bool isEnableAudit)
        {
            this.IsEnableAudit = isEnableAudit;
            return this;
        }

        public AuditConfig IgnoreSameValue(bool isIgnoreSameValue)
        {
            this.IsIgnoreSameValue = isIgnoreSameValue;
            return this;
        }

        public static AuditConfig Default => new AuditConfig();

    }
}
