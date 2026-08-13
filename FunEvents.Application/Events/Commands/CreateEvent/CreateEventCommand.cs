using FunEvents.Application.Common.Results;
using MediatR;

namespace FunEvents.Application.Events.Commands.CreateEvent
{
    public sealed record CreateEventCommand(string Code, string Name, DateTime Date, int Capacity) : IRequest<Result<CreateEventResult>>;

}
