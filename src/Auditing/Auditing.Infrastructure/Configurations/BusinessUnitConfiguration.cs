using Auditing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure.Configurations
{
    public class BusinessUnitConfiguration:IEntityTypeConfiguration<BusinessUnit>
    {
        public void Configure(EntityTypeBuilder<BusinessUnit> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasMaxLength(32).ValueGeneratedOnAdd();
            builder.Property(c => c.OrgCode).HasMaxLength(30).IsRequired();
            builder.Property(c => c.OrgName).HasMaxLength(30).IsRequired();
            builder.Property(c => c.ParentOrg).HasMaxLength(32);
            builder.Property(c => c.IsActive).HasMaxLength(2).IsRequired();
            builder.Property(c => c.CreatedBy).HasMaxLength(10);
        }

    }
}
