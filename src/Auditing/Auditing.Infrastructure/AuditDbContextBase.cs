using Auditing.Domain;
using Auditing.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auditing.Infrastructure
{
    public class AuditDbContextBase : DbContext, IAuditStorage
    {
        private readonly AuditConfig _auditConfig;
        private List<AuditEntry> _auditEntries;

        public DbSet<AuditLog> AuditLog { get; set; }

        public AuditDbContextBase(DbContextOptions options, AuditConfig auditConfig) : base(options)
        {
            _auditConfig = auditConfig;
        }

        public virtual Task BeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            _auditEntries = new List<AuditEntry>();
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.State == EntityState.Detached || entityEntry.State == EntityState.Unchanged)
                    continue;
                if (entityEntry.Entity.GetType() == typeof(AuditLog))
                    continue;
                if (_auditConfig.EntityFilters.Any(x => x(entityEntry)))
                    continue;

                var auditEntry = new AuditEntry(entityEntry, _auditConfig);
                _auditEntries.Add(auditEntry);
            }

            return Task.CompletedTask;
        }

        public virtual Task AfterSaveChanges()
        {
            if (_auditEntries == null || !_auditEntries.Any())
                return Task.CompletedTask;

            foreach (var auditEntry in _auditEntries)
            {
                auditEntry.UpdateTemporaryProperties();
            }

            //扩展字段
            foreach (var auditEntry in _auditEntries)
            {
                if (_auditConfig.PropertyEnrichers != null && _auditConfig.PropertyEnrichers.Any())
                {
                    _auditConfig.PropertyEnrichers.ForEach(x => x(auditEntry.ExtraData));
                }
            }

            //保存审计日志
            var auditLogs = _auditEntries.Select(x => x.AsAuditLog()).ToArray();
            if (!_auditConfig.AuditStorages.Any())
                _auditConfig.AuditStorages.Add(this);
            _auditConfig.AuditStorages.ForEach(
                auditStorage => auditStorage.SaveAuditLogs(auditLogs)
            );

            return Task.CompletedTask;
        }

        public override int SaveChanges()
        {
            BeforeSaveChanges().Wait();
            var result = base.SaveChanges();
            BeforeSaveChanges().Wait();
            return result;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            BeforeSaveChanges().Wait();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            BeforeSaveChanges().Wait();
            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await BeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await AfterSaveChanges();
            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AuditLogConfiguration());
        }

        void IAuditStorage.SaveAuditLogs(params AuditLog[] auditLogs)
        {
            AuditLog.AddRange(auditLogs);
            base.SaveChangesAsync();
        }
    }
}
