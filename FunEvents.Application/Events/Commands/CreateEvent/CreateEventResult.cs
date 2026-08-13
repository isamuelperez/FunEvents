using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.CreateEvent
{
    public sealed record CreateEventResult(
         Guid Id,
    string Code,
    string Name,
    DateTime Date,
    int Capacity,
    int AvailableTickets);
}
