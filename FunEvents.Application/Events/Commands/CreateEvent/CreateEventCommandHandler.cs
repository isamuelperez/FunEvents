using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Application.Events.Commands.CreateEvent
{
    public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Result<CreateEventResult>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<CreateEventResult>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var exists = await _eventRepository.ExistsByCodeAsync(
           request.Code,
           cancellationToken);

            if (exists)
            {
                return Result<CreateEventResult>.Failure(Error(
                    "Event.DuplicateCode", 
                    "Ya existe un evento con ese código.")
                );
            }

            try
            {
                var result = await _unitOfWork.ExecuteInTransactionAsync<CreateEventResult>(async ct =>
                {
                    var eventEntity = new Event
                    {
                        Id = Guid.NewGuid(),
                        Code = request.Code.Trim(),
                        Name = request.Name.Trim(),
                        Date = request.Date,
                        Capacity = request.Capacity,
                        AvailableTickets = request.Capacity
                    };

                    await _eventRepository.AddAsync(eventEntity, ct);

                    await _unitOfWork.SaveChangesAsync(ct);

                    return new CreateEventResult(
                        eventEntity.Id,
                        eventEntity.Code,
                        eventEntity.Name,
                        eventEntity.Date,
                        eventEntity.Capacity,
                        eventEntity.AvailableTickets);

                }, cancellationToken);

                return Result<CreateEventResult>.Success(result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<CreateEventResult>.Failure(Error("Event.ConcurrencyConflict", "Ocurrio un error"));
            }
            catch
            {
                throw;
            }


            
        }
    }
}
