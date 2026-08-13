namespace FunEvents.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int Capacity { get; set; }
        public int AvailableTickets { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = [];

        // concurrencia optimista.
        public byte[] RowVersion { get; set; } = [];
    }
}
