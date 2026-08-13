using FluentValidation;

namespace FunEvents.Application.Reservations.Commands.CreateReservation
{
    public sealed class CreateReservationValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationValidator()
        {
            RuleFor(x => x.EventCode)
                .NotEmpty()
                .WithErrorCode("EventCode.Required")
                .WithMessage("El código del evento es obligatorio.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithErrorCode("UserId.Required")
                .WithMessage("El usuario es obligatorio.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithErrorCode("Quantity.Invalid")
                .WithMessage("La cantidad debe ser mayor que cero.");

            RuleFor(x => x.Quantity)
                .LessThanOrEqualTo(10)
                .WithErrorCode("Quantity.Exceeded")
                .WithMessage("No se pueden reservar más de 10 entradas.");
        }
    }
}
