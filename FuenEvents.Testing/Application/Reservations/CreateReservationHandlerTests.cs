using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Reservations.Commands.CreateReservation;
using FunEvents.Domain.Entities;
using FunEvents.Domain.Enums;
using Xunit;

namespace FuenEvents.Testing.Application.Reservations;

public class CreateReservationHandlerTests
{
    private class FakeUserRepository : IUserRepository
    {
        public Func<System.Guid, CancellationToken, Task<User?>>? GetByIdImpl;
        public Task<User?> GetByIdAsync(System.Guid id, CancellationToken cancellationToken) => GetByIdImpl != null ? GetByIdImpl(id, cancellationToken) : Task.FromResult<User?>(null);
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult<User?>(null);
        public Task AddAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> ExistsAsync(System.Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private class FakeEventRepository : IEventRepository
    {
        public Func<string, CancellationToken, Task<Event?>>? GetByCodeImpl;
        public Func<string, CancellationToken, Task<bool>>? ExistsImpl;
        public Func<Event, CancellationToken, Task>? AddImpl;

        public Task AddAsync(Event eventEntity, CancellationToken cancellationToken) => AddImpl != null ? AddImpl(eventEntity, cancellationToken) : Task.CompletedTask;
        public Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult((IReadOnlyList<Event>)new System.Collections.Generic.List<Event>());
        public Task<Event?> GetByCodeAsync(string code, CancellationToken cancellationToken) => GetByCodeImpl != null ? GetByCodeImpl(code, cancellationToken) : Task.FromResult<Event?>(null);
        public Task<Event?> GetByIdAsync(System.Guid id, CancellationToken cancellationToken) => Task.FromResult<Event?>(null);
        public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken) => ExistsImpl != null ? ExistsImpl(code, cancellationToken) : Task.FromResult(false);
        public void Remove(Event eventEntity) => throw new System.NotImplementedException();
        public void Update(Event eventEntity) => throw new System.NotImplementedException();
    }

    private class FakeReservationRepository : IReservationRepository
    {
        public Func<Reservation, CancellationToken, Task>? AddImpl;
        public Task AddAsync(Reservation reservation, CancellationToken cancellationToken) => AddImpl != null ? AddImpl(reservation, cancellationToken) : Task.CompletedTask;
        public Task<bool> ExistsForUserAndEventAsync(System.Guid userId, System.Guid eventId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> HasReservationsAsync(System.Guid eventId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<Reservation?> GetByIdWithDetailsAsync(System.Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Reservation?>(null);
        public Task<IReadOnlyList<Reservation>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) => Task.FromResult((IReadOnlyList<Reservation>)new System.Collections.Generic.List<Reservation>());
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<T> ExecuteInTransactionAsync<T>(System.Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) => operation(cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        var userRepo = new FakeUserRepository { GetByIdImpl = (id, ct) => Task.FromResult<User?>(null) };
        var eventRepo = new FakeEventRepository();
        var reservationRepo = new FakeReservationRepository();
        var uow = new FakeUnitOfWork();

        var handler = new CreateReservationHandler(userRepo, eventRepo, reservationRepo, uow);

        var command = new CreateReservationCommand("EVT", System.Guid.NewGuid(), 1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CreatesReservation()
    {
        var userId = System.Guid.NewGuid();

        var userRepo = new FakeUserRepository { GetByIdImpl = (id, ct) => Task.FromResult<User?>(new User { Id = userId, Name = "Test", Email = "a@b.com" }) };

        var evt = new Event { Id = System.Guid.NewGuid(), Code = "EVT", Name = "E", Date = System.DateTime.UtcNow, Capacity = 10, AvailableTickets = 5 };

        var eventRepo = new FakeEventRepository { GetByCodeImpl = (code, ct) => Task.FromResult<Event?>(evt) };

        var reservationAdded = 0;
        var reservationRepo = new FakeReservationRepository { AddImpl = (r, ct) => { reservationAdded++; return Task.CompletedTask; } };

        var uow = new FakeUnitOfWork();

        var handler = new CreateReservationHandler(userRepo, eventRepo, reservationRepo, uow);

        var command = new CreateReservationCommand("EVT", userId, 2);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, reservationAdded);
    }
}
