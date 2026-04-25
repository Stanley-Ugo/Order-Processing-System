using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.IdempotencyKey)
               .HasMaxLength(100)
               .IsRequired();

            builder.HasIndex(o => o.IdempotencyKey)
               .IsUnique();

            builder.Property(o => o.TotalAmount)
               .HasPrecision(18, 2);

            builder.Property(o => o.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

            builder.HasMany(o => o.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
