using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.UpdateEvent
{
    public sealed class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<CreateEventResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventCommandHandler(
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<CreateEventResult>> Handle(
        UpdateEventCommand request,
        CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (eventEntity is null)
            {
                return Result<CreateEventResult>.Failure(Error(
                    "",
                    "El evento no existe."));
            }

            var reservedTickets =
                eventEntity.Capacity - eventEntity.AvailableTickets;

            if (request.Capacity < reservedTickets)
            {
                return Result<CreateEventResult>.Failure(Error(
                    "",
                    $"La capacidad no puede ser menor que las entradas ya reservadas ({reservedTickets})."));
            }

            eventEntity.Name = request.Name.Trim();
            eventEntity.Date = request.Date;

            var capacityDifference =
                request.Capacity - eventEntity.Capacity;

            eventEntity.Capacity = request.Capacity;
            eventEntity.AvailableTickets += capacityDifference;

            try
            {
                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<CreateEventResult>.Failure(Error(
                    "",
                    "El evento fue modificado por otro proceso. Intente nuevamente."));
            }

            return Result<CreateEventResult>.Success(
                new CreateEventResult(
                    eventEntity.Id,
                    eventEntity.Code,
                    eventEntity.Name,
                    eventEntity.Date,
                    eventEntity.Capacity,
                    eventEntity.AvailableTickets));
        }
    }
}
