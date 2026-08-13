using System;
using MediatR;
using FunEvents.Application.Common.Results;

namespace FunEvents.Application.Reservations.Commands.CreateReservation
{
    public sealed record CreateReservationCommand(
        string EventCode,
        Guid UserId,
        int Quantity
    ) : IRequest<Result<CreateReservationResult>>;
}
