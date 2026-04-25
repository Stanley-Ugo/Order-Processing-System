using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Domain.Common;

namespace OrderProcessingSystem.Infrastructure.Persistence.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
               .HasMaxLength(200)
               .IsRequired();

            builder.Property(x => x.Content)
               .IsRequired();

            builder.HasIndex(x => new { x.ProcessedOn, x.OccurredOn });
        }
    }
}
