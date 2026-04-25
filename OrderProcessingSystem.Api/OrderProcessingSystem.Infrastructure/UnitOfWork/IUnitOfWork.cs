using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace OrderProcessingSystem.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
