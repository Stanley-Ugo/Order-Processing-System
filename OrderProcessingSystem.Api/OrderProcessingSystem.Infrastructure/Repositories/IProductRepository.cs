using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken ct);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct);
    }
}
