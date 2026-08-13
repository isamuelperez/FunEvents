using FunEvents.Application.Common.Results;
using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.UpdateEvent
{
    public sealed record UpdateEventCommand(
        Guid Id,
        string Name,
        DateTime Date,
        int Capacity
    ) : IRequest<Result<CreateEventResult>>;
}
