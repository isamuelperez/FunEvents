using FunEvents.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.DeleteEvent
{
    public record DeleteEventCommand(Guid Id) : IRequest<Result<bool>>;
}
