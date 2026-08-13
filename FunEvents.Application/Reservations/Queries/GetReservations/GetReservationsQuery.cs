using FunEvents.Application.Common.Results;
using FunEvents.Application.Reservations.Queries.GetByReservation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Queries.GetReservations
{
    public sealed record GetReservationsQuery
    : IRequest<Result<IReadOnlyList<ReservationResponse>>>;
}
