using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Domain.Entities;
using OrderProcessingSystem.Infrastructure.Persistence;

namespace OrderProcessingSystem.Infrastructure.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) => _db = db;

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);

        public Task<Order?> GetByIdempotencyKeyAsync(string key, CancellationToken ct) =>
            _db.Orders.FirstOrDefaultAsync(o => o.IdempotencyKey == key, ct);

        public async Task AddAsync(Order order, CancellationToken ct) =>
            await _db.Orders.AddAsync(order, ct);
    }
}
