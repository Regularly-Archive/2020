using Auditing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auditing.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Address).HasMaxLength(200);
            builder.Property(c => c.Name).HasMaxLength(10).IsRequired();
            builder.Property(c => c.Tel).HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(20).IsRequired();
        }
    }
}