using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.CreateEvent
{
    public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La fecha del evento debe ser futura.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("La capacidad debe ser mayor que cero.");
        }
    }
}
