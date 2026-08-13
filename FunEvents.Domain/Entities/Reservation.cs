using FunEvents.Domain.Enums;

namespace FunEvents.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public ReservationStatus Status { get; set; }

        public User User { get; set; } = null!;

        public Event Event { get; set; } = null!;
    }
}
