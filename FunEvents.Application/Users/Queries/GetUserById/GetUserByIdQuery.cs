using FunEvents.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserResponse>>;

    public record UserResponse(
        Guid Id,
        string Name,
        string Email
    );
}
