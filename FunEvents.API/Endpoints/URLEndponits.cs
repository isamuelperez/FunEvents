using Microsoft.AspNetCore.Http.HttpResults;

namespace FunEvents.API.Endpoints
{
    public static class URLEndponits
    {

        public static IEndpointRouteBuilder MapURLEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/url").WithTags("Url");

            group.MapGet("/url", GetUrl);

          
            return app;
        }

        public static string GetUrl()
        {
            return "https://winnergroup.com/";
        }

    }
}
