using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Events.Commands.UpdateEvent
{
    public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La fecha debe ser futura.");

            RuleFor(x => x.Capacity)
                 .GreaterThan(0);

        }
    }
}
