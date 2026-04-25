using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderProcessingSystem.Infrastructure.Persistence;
using System.Data;

namespace OrderProcessingSystem.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db) => _db = db;

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken ct) =>
            _db.Database.BeginTransactionAsync(isolationLevel, ct);

        public Task<int> SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);
    }
}
