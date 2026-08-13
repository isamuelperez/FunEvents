using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FunEvents.Infrastructure.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events", table =>
            {
                table.HasCheckConstraint(
                    "CK_Events_Capacity",
                    "[Capacity] > 0");

                table.HasCheckConstraint(
                    "CK_Events_AvailableTickets",
                    "[AvailableTickets] >= 0");

                table.HasCheckConstraint(
                    "CK_Events_AvailableTickets_Capacity",
                    "[AvailableTickets] <= [Capacity]");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(e => e.AvailableTickets)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
