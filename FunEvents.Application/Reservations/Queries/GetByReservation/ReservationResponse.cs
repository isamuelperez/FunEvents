using FunEvents.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Queries.GetByReservation
{
    public sealed record ReservationResponse(
    Guid Id,
    int Quantity,
    DateTime CreatedAt,
    ReservationStatus Status,
    UserResponse User,
    EventResponse Event);

    public sealed record UserResponse(
        Guid Id,
        string Name,
        string Email);

    public sealed record EventResponse(
        Guid Id,
        string Code,
        string Name,
        DateTime Date
        );
}
