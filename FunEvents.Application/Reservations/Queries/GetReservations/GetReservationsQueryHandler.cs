using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Application.Reservations.Queries.GetByReservation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Queries.GetReservations
{
    internal class GetReservationsQueryHandler(IReservationRepository reservationRepository) : IRequestHandler<GetReservationsQuery,Result<IReadOnlyList<ReservationResponse>>>
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;

        public async Task<Result<IReadOnlyList<ReservationResponse>>> Handle(
        GetReservationsQuery request,
        CancellationToken cancellationToken)
        {
            var reservations =
                await _reservationRepository.GetAllWithDetailsAsync(
                    cancellationToken);

            var response = reservations
                .Select(reservation =>
                    new ReservationResponse(
                        reservation.Id,
                        reservation.Quantity,
                        reservation.CreatedAt,
                        reservation.Status,
                        new UserResponse(
                            reservation.User.Id,
                            reservation.User.Name,
                            reservation.User.Email),
                        new EventResponse(
                            reservation.Event.Id,
                            reservation.Event.Code,
                            reservation.Event.Name,
                            reservation.Event.Date)))
                .ToList();

            return Result<IReadOnlyList<ReservationResponse>>.Success(response);
        }

    }
}
