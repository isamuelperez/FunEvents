using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Queries.GetEvents
{
    public class GetEventsQueryHandler(IEventRepository eventRepository) : IRequestHandler<GetEventsQuery, IReadOnlyList<CreateEventResult>>
    {
        private readonly IEventRepository _eventRepository = eventRepository;

        public async Task<IReadOnlyList<CreateEventResult>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetAllAsync(cancellationToken);

            return events
                .Select(
                e => new CreateEventResult(
                    e.Id,
                    e.Code,
                    e.Name,
                    e.Date,
                    e.Capacity,
                    e.AvailableTickets)
                )
                .ToList();
        }
    }
}
