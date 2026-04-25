using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Sku)
               .HasMaxLength(50)
               .IsRequired();

            builder.HasIndex(p => p.Sku).IsUnique();

            builder.Property(p => p.Name)
               .HasMaxLength(200)
               .IsRequired();

            builder.Property(p => p.UnitPrice)
               .HasPrecision(18, 2);

            // SQL Server optimistic concurrency
            builder.Property(p => p.RowVersion)
               .IsRowVersion();
        }
    }
}
