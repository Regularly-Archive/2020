using Auditing.Domain;
using Auditing.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Auditing.Infrastructure
{
    public class CustomerContext : AuditDbContextBase
    {
        public DbSet<Customer> Customer { get; set; }

        public CustomerContext(DbContextOptions options,AuditConfig auditConfig) : base(options, auditConfig)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new CustomerConfiguration());
        }
    }
}
