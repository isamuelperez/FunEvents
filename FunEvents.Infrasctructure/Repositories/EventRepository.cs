using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public sealed class EventRepository(FunEventsDbContext context) : IEventRepository
    {
        private readonly FunEventsDbContext _context = context;
        public async Task<Event?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
        {
            return await _context.Events
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<Event?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken)
        {
            return await _context.Events
                .FirstOrDefaultAsync(
                    x => x.Code == code,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<Event>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken)
        {
            return await _context.Events
                .AnyAsync(
                    x => x.Code == code,
                    cancellationToken);
        }

        public async Task AddAsync(
            Event eventEntity,
            CancellationToken cancellationToken)
        {
            await _context.Events.AddAsync(
                eventEntity,
                cancellationToken);
        }

        public void Update(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
        }

        public void Remove(Event eventEntity)
        {
            _context.Events.Remove(eventEntity);
        }
    
        
    }
}
