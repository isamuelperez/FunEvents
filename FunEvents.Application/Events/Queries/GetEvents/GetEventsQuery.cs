using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Queries.GetEvents
{
    public sealed record GetEventsQuery : IRequest<IReadOnlyList<CreateEventResult>>;

}
