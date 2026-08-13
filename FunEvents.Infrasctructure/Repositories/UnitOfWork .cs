using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FunEvents.Infrastructure.Repositories
{
    internal class UnitOfWork(FunEventsDbContext context) : IUnitOfWork
    {
        private readonly FunEventsDbContext _context = context;

        private IDbContextTransaction? _transaction;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database
                    .BeginTransactionAsync(cancellationToken);

                var result = await operation(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return result;
            });
        }
    }
}
