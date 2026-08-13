using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Reservations.Queries.GetByReservation
{
    public sealed class GetReservationByIdQueryHandler(
        IReservationRepository reservationRepository
        ) : IRequestHandler<GetReservationByIdQuery, Result<ReservationResponse>>
    {
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private static ResultError Error(string code, string message) => new ResultError(code, message);
        public async Task<Result<ReservationResponse>> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
        {
            var reservation =
                await _reservationRepository.GetByIdWithDetailsAsync(
                    request.Id,
                    cancellationToken);

            if (reservation is null)
            {
                return Result<ReservationResponse>.Failure(Error(
                    "Reservation.NotFound", 
                    "No se enccontro la reserva")
                );
            }

            var response = new ReservationResponse(
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
                    reservation.Event.Date
                ));

            return Result<ReservationResponse>.Success(response);
        }
    }
}
