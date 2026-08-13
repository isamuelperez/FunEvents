using FunEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Abstractions.Persistence
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

        Task<Event?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Event>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken);

        Task AddAsync(
            Event eventEntity,
            CancellationToken cancellationToken);

        void Update(Event eventEntity);

        void Remove(Event eventEntity);
    }
}
