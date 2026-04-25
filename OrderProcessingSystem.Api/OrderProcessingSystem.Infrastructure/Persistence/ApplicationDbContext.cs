using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderProcessingSystem.Application.Common.Interfaces;
using OrderProcessingSystem.Domain.Common;
using OrderProcessingSystem.Domain.Entities;
using System.Data;
using System.Text.Json;

namespace OrderProcessingSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // Convert domain events to outbox messages before saving
            var domainEntities = ChangeTracker
               .Entries<Entity>()
               .Where(x => x.Entity.DomainEvents.Any())
               .Select(x => x.Entity);

            var domainEvents = domainEntities
               .SelectMany(x => x.DomainEvents)
               .ToList();

            domainEntities.ToList().ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().Name,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = domainEvent.OccurredOn
                });
            }

            return await base.SaveChangesAsync(ct);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }
    }
}
