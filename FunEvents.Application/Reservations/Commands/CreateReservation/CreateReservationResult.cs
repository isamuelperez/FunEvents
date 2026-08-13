using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Commands.CreateReservation
{
    public sealed record CreateReservationResult(
        Guid ReservationId,
        string EventCode,
        Guid UserId,
        int Quantity,
        string Status
    );
}
