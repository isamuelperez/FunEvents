using FunEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Abstractions.Persistence
{
    public interface IReservationRepository
    {
        Task AddAsync( Reservation reservation, CancellationToken cancellationToken);

        Task<bool> ExistsForUserAndEventAsync( Guid userId, Guid eventId, CancellationToken cancellationToken);
        Task<bool> HasReservationsAsync(Guid eventId, CancellationToken cancellationToken);

        Task<Reservation?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Reservation>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);
    }
}
