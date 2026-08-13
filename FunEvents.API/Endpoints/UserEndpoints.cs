using FluentValidation;
using FunEvents.Application.Events.Commands.CreateEvent;
using FunEvents.Application.Users.Commands.CreateUser;
using FunEvents.Application.Users.Queries.GetUserById;
using MediatR;

namespace FunEvents.API.Endpoints
{
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/users").WithTags("Users");

            group.MapGet("/{id:guid}", GetUser);

            group.MapPost("/", Create);

            return app;
        }

        private static async Task<IResult> GetUser(Guid id, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.NotFound(new
                {
                    Response = result.Errors
                });
            }

            return Results.Ok(result.Value);

        }

        private static async Task<IResult> Create(CreateUserCommand command, IValidator<CreateUserCommand> validator, ISender sender, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(command, cancellationToken);

            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new
                {
                    Response = result.Errors
                });
            }
            return Results.Created($"/api/users/{result?.Value?.Id}", result);


        }
    }
}
