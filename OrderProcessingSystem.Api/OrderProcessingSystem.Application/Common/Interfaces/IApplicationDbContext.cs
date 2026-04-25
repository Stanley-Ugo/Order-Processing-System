using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Domain.Common;
using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<Product> Products { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(
            System.Data.IsolationLevel isolationLevel,
            CancellationToken cancellationToken);
    }
}
