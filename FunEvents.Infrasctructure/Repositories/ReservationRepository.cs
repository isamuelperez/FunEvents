using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public sealed class ReservationRepository(FunEventsDbContext context) : IReservationRepository
    {
        private readonly FunEventsDbContext _context = context;

        public async Task AddAsync(
            Reservation reservation,
            CancellationToken cancellationToken)
        {
            await _context.Reservations.AddAsync(
                reservation,
                cancellationToken
            );
        }

        public async Task<bool> ExistsForUserAndEventAsync(
            Guid userId,
            Guid eventId,
            CancellationToken cancellationToken)
        {
            return await _context
                .Reservations
                .AnyAsync(
                    r => r.UserId == userId &&
                         r.EventId == eventId,
                    cancellationToken
                );
        }

        public async Task<bool> HasReservationsAsync(
            Guid eventId,
            CancellationToken cancellationToken)
        {
            return await _context.Reservations
                .AnyAsync(
                    x => x.EventId == eventId,
                    cancellationToken);
        }

        public async Task<Reservation?> GetByIdWithDetailsAsync(
    Guid id,
    CancellationToken cancellationToken = default)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Event)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<Reservation>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Event)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
