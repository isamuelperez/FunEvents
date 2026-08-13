using System;
using System.Threading;
using System.Threading.Tasks;
using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Infrastructure.Data
{
    public class FunEventsDbContext : DbContext
    {
        public FunEventsDbContext(
        DbContextOptions<FunEventsDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Event> Events => Set<Event>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FunEventsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public async Task EnsureSeedDataAsync(CancellationToken cancellationToken = default)
        {
            // Seed a deterministic user and event so test clients can rely on known ids/codes.
            if (!await Users.AnyAsync(cancellationToken))
            {
                var seededUser = new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Seed User",
                    Email = "user@funevents.local"
                };

                await Users.AddAsync(seededUser, cancellationToken);
            }

            if (!await Events.AnyAsync(cancellationToken))
            {
                var seededEvent = new Event
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "EVT-001",
                    Name = "Seeded Concert",
                    Date = DateTime.UtcNow.AddDays(30),
                    Capacity = 100,
                    AvailableTickets = 100
                };

                await Events.AddAsync(seededEvent, cancellationToken);
            }

            await SaveChangesAsync(cancellationToken);
        }
    }
}
