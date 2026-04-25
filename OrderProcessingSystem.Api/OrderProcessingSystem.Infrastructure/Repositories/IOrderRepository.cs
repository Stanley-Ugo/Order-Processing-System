using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Order?> GetByIdempotencyKeyAsync(string key, CancellationToken ct);
        Task AddAsync(Order order, CancellationToken ct);
    }
}
