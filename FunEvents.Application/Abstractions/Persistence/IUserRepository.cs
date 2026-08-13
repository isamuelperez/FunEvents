using FunEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<User?> GetByEmailAsync(
    string email,
    CancellationToken cancellationToken = default);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
