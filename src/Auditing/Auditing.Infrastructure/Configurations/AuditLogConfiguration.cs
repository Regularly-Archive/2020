using Auditing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auditing.Infrastructure.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.TableName).HasMaxLength(30).IsRequired();
            builder.Property(c => c.OldValues).HasMaxLength(500);
            builder.Property(c => c.NewValues).HasMaxLength(500);
            builder.Property(c => c.ExtraData).HasMaxLength(500);
            builder.Property(c => c.CreatedBy).HasMaxLength(30);
        }
    }
}