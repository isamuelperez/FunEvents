using FunEvents.Application.Common.Results;
using FunEvents.Application.Events.Commands.CreateEvent;
using MediatR;

namespace FunEvents.Application.Events.Queries.GetEvenByCode
{
    public sealed record  GetEventByCodeQuery(
        string Code
    ) : IRequest<Result<CreateEventResult>>;
}
