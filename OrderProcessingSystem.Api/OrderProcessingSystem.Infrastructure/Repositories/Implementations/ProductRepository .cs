using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Domain.Entities;
using OrderProcessingSystem.Infrastructure.Persistence;

namespace OrderProcessingSystem.Infrastructure.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) => _db = db;

        public Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken ct) =>
            _db.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);

        public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct) =>
            _db.Products.FirstOrDefaultAsync(p => p.Sku == sku, ct);
    }
}
