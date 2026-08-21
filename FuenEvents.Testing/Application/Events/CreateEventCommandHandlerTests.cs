using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Events.Commands.CreateEvent;
using FunEvents.Domain.Entities;
using Xunit;

namespace FuenEvents.Testing.Application.Events;

public class CreateEventCommandHandlerTests
{
    private class FakeEventRepository : IEventRepository
    {
        public Func<string, CancellationToken, Task<bool>>? ExistsImpl;
        public Func<Event, CancellationToken, Task>? AddImpl;

        public Task AddAsync(Event eventEntity, CancellationToken cancellationToken)
            => AddImpl != null ? AddImpl(eventEntity, cancellationToken) : Task.CompletedTask;

        public Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult((IReadOnlyList<Event>)new List<Event>());

        public Task<Event?> GetByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult<Event?>(null);

        public Task<Event?> GetByIdAsync(System.Guid id, CancellationToken cancellationToken) => Task.FromResult<Event?>(null);

        public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken) => ExistsImpl != null ? ExistsImpl(code, cancellationToken) : Task.FromResult(false);

        public void Remove(Event eventEntity) => throw new System.NotImplementedException();

        public void Update(Event eventEntity) => throw new System.NotImplementedException();
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        public Task<T> ExecuteInTransactionAsync<T>(System.Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
            => operation(cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenEventCodeExists_ReturnsFailure()
    {
        var repo = new FakeEventRepository { ExistsImpl = (code, ct) => Task.FromResult(true) };
        var uow = new FakeUnitOfWork();

        var handler = new CreateEventCommandHandler(repo, uow);

        var command = new CreateEventCommand("CODE", "Name", System.DateTime.UtcNow, 10);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CreatesEvent()
    {
        var added = 0;
        var repo = new FakeEventRepository
        {
            ExistsImpl = (code, ct) => Task.FromResult(false),
            AddImpl = (e, ct) => { added++; return Task.CompletedTask; }
        };

        var uow = new FakeUnitOfWork();

        var handler = new CreateEventCommandHandler(repo, uow);

        var command = new CreateEventCommand("CODE2", "Event name", System.DateTime.UtcNow.AddDays(1), 5);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, added);
    }
}
