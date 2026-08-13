using FunEvents.Application.Common.Results;
using FunEvents.Application.Users.Queries.GetUserById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Users.Commands.CreateUser
{
    public record class CreateUserCommand(
        string Name,
        string Email
    ) : IRequest<Result<UserResponse>>;
}
