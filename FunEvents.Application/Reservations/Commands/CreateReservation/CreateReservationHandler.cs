using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Domain.Entities;
using FunEvents.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Application.Reservations.Commands.CreateReservation
{
    public sealed class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<CreateReservationResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReservationHandler(
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IReservationRepository reservationRepository,
            IUnitOfWork unitOfWork
            )
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<CreateReservationResult>> Handle(
            CreateReservationCommand command,
            CancellationToken cancellationToken = default
            )
        {
            var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

            if (user is null)
                return Result<CreateReservationResult>.Failure(Error("User.NotFound", "El usuario no existe."));

            var evento = await _eventRepository.GetByCodeAsync(
                command.EventCode,
                cancellationToken);

            if (evento is null)
                return Result<CreateReservationResult>.Failure(Error("Event.NotFound", "El evento no existe."));

            if (evento.AvailableTickets < command.Quantity)
                return Result<CreateReservationResult>.Failure(Error("Event.InsufficientTickets", "No hay suficientes entradas disponibles."));

            try
            {
                var result = await _unitOfWork.ExecuteInTransactionAsync<CreateReservationResult>(async ct =>
                {
                    evento.AvailableTickets -= command.Quantity;

                    var reservation = new Reservation
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        EventId = evento.Id,
                        Quantity = command.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        Status = ReservationStatus.Confirmed
                    };

                    await _reservationRepository.AddAsync(reservation, ct);
                    await _unitOfWork.SaveChangesAsync(ct);

                    return new CreateReservationResult(
                        reservation.Id,
                        evento.Code,
                        user.Id,
                        reservation.Quantity,
                        reservation.Status.ToString());
                }, cancellationToken);

                return Result<CreateReservationResult>.Success(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<CreateReservationResult>.Failure(Error("Reservation.ConcurrencyConflict", "Ocurrio un error. Intente nuevamente."));
            }
        }

    }
}
