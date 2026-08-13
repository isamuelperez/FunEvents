using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.DeleteEvent
{
    public sealed class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventCommandHandler(
            IEventRepository eventRepository,
            IReservationRepository reservationRepository,
            IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<bool>> Handle(
        DeleteEventCommand request,
        CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (eventEntity is null)
            {
                return Result<bool>.Failure(Error(
                    "",
                    "El evento no existe."));
            }

            var hasReservations =
                await _reservationRepository.HasReservationsAsync(
                    eventEntity.Id,
                    cancellationToken);

            if (hasReservations)
            {
                return Result<bool>.Failure(Error(
                    "",
                    "No se puede eliminar un evento que tiene reservas."));
            }

            _eventRepository.Remove(eventEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<bool>.Failure(Error(
                    "",
                    "El evento fue modificado por otro proceso."));
            }

            return Result<bool>.Success(true);
        }
    }
}
