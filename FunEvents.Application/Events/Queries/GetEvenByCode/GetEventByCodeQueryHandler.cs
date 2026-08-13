using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Queries.GetEvenByCode
{
    internal class GetEventByCodeQueryHandler : IRequestHandler<GetEventByCodeQuery, Result<CreateEventResult>>
    {
        private readonly IEventRepository _eventRepository;

        public GetEventByCodeQueryHandler(
        IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<CreateEventResult>> Handle(GetEventByCodeQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByCodeAsync(request.Code, cancellationToken);

            if (eventEntity is null)
            {
                return Result<CreateEventResult>.Failure(Error(
                    "Event.NotFound",
                    "El evento no existe.")
                );
            }

            return Result<CreateEventResult>.Success(
                new CreateEventResult(
                    eventEntity.Id,
                    eventEntity.Code,
                    eventEntity.Name,
                    eventEntity.Date,
                    eventEntity.Capacity,
                    eventEntity.AvailableTickets)
                );
        }
    }
}
