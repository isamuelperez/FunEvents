using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public sealed class UserRepository(FunEventsDbContext context) : IUserRepository
    {
        private readonly FunEventsDbContext _context = context;
        public async Task<User?> GetByIdAsync(
            Guid id, CancellationToken cancellationToken)
        {
            return await _context
                .Users
                .FirstOrDefaultAsync(
                    u => 
                    u.Id == id,
                    cancellationToken
                );
        }

        public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Email == email,
                    cancellationToken);
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(
                user,
                cancellationToken);
        }

        public async Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(
                    x => x.Id == id,
                    cancellationToken);
        }
    }
}
