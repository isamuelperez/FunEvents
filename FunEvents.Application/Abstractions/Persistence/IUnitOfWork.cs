using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
        Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default);
    }
}
