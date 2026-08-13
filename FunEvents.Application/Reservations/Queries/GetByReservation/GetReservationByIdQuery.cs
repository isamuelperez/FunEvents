using FunEvents.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Queries.GetByReservation
{
    public sealed record GetReservationByIdQuery(Guid Id) : IRequest<Result<ReservationResponse>>;
}
