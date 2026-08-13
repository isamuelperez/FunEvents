using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Users.Queries.GetUserById
{
    public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("El Id del usuario es obligatorio.");
        }
    }
}
