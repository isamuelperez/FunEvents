namespace FunEvents.API.Endpoints
{
    using FluentValidation;
    using FunEvents.Application.Reservations.Commands.CreateReservation;
    using FunEvents.Application.Reservations.Queries.GetByReservation;
    using FunEvents.Application.Reservations.Queries.GetReservations;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class ReservationEndpoints
    {
        public static IEndpointRouteBuilder MapReservationEndpoints(
        this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/reservations").WithTags("Reservations");

            group.MapPost("/", CreateReservation);

            group.MapGet("/", GetAll);

            group.MapGet("/{id:guid}", GetById);

            return app;
        }

        private static async Task<IResult> CreateReservation(
            CreateReservationCommand command,
            IValidator<CreateReservationCommand> validator,
            ISender sender,
            CancellationToken cancellationToken
            )
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new
                {
                    Response = result.Errors
                });
            }

            return Results.Created($"/api/reservations/{result.Value!.ReservationId}", result.Value);
        }

        private static async Task<IResult> GetAll(
        ISender sender,
        CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new GetReservationsQuery(),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(new
            {
                StatusCode = 200,
                Data = result.Value
            });
        }

        private static async Task<IResult> GetById(
            Guid id,
            ISender sender,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new GetReservationByIdQuery(id),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.NotFound(result.Errors);
            }

            return Results.Ok(result.Value);
        }
    }


}
