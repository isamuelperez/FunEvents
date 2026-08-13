using FluentValidation;
using FunEvents.Application.Events.Commands.CreateEvent;
using FunEvents.Application.Events.Commands.DeleteEvent;
using FunEvents.Application.Events.Commands.UpdateEvent;
using FunEvents.Application.Events.Queries.GetEvenByCode;
using FunEvents.Application.Events.Queries.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FunEvents.API.Endpoints
{
    public static class EventEndpoints
    {
        public static IEndpointRouteBuilder MapEventEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/events").WithTags("Events");

            group.MapGet("/", GetEvents);

            group.MapGet("/code/{code}", GetEventByCode);
            
            group.MapPost("/", CreateEvent);
            
            group.MapPut("/{id:guid}", UpdateEvent);
            
            group.MapDelete("/{id:guid}", DeleteEvent);
          

            return app;
        }

        private static async Task<IResult> GetEvents(ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetEventsQuery(), cancellationToken);

            return Results.Ok(result);
        }

        private static async Task<IResult> GetEventByCode(string code, ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetEventByCodeQuery(code), cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.NotFound(new
                {
                    Response = result.Errors
                });
            }

            return Results.Ok(result.Value);
        }

        

        private static async Task<IResult> CreateEvent(
            CreateEventCommand command,
            IValidator<CreateEventCommand> validator,
            ISender sender,
            CancellationToken cancellationToken
            )
        {
            var validation = await validator.ValidateAsync(command, cancellationToken);

            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            var result = await sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return Results.Conflict(new
                {
                    Response = result.Errors
                });
            }

            return Results.Created($"/api/events/{result.Value!.Id}", result.Value);
        }

       
       private static async Task<IResult> UpdateEvent(
           Guid id,
           [FromBody] UpdateEventCommand command,
           IValidator<UpdateEventCommand> validator,
           ISender sender,
           CancellationToken cancellationToken
           )
       {
           if (id != command.Id)
           {
               return Results.BadRequest(new
               {
                   message = "El Id de la ruta no coincide con el Id del recurso."
               });
           }

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

           return Results.Ok(result.Value);
       }



       

        private static async Task<IResult> DeleteEvent(Guid id, ISender sender, CancellationToken cancellationToken)
      {
          var result = await sender.Send(new DeleteEventCommand(id), cancellationToken);

          if (!result.IsSuccess)
          {
              return Results.BadRequest(new
              {
                  Response = result.Errors
              });
          }

          return Results.NoContent();
      }
      
    }
}
