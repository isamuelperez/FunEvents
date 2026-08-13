using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Users.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("El nombre es obligatorio.")
           .MaximumLength(100)
           .WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El email es obligatorio.")
                .EmailAddress()
                .WithMessage("El email no tiene un formato válido.")
                .MaximumLength(100)
                .WithMessage("El email no puede superar los 100 caracteres.");
        }
    }
}
