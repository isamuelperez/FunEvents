using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunEvents.Infrastructure.Data.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Event)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.EventId);
            builder.HasIndex(x => x.UserId);
        }
    }
}
