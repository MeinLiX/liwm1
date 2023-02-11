using Application.Futures.GamesFuture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class GameRoutes
{
    public static WebApplication InitGameRoutes(this WebApplication web)
        => web.InitGameRoute();

    private static WebApplication InitGameRoute(this WebApplication web)
    {
        web.MapPost("/games", async (IMediator mediator, [FromQuery] int? id, int? start, int? count) =>
        {
            var request = new GameRequest
            {
                id = id,
                start = start,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        })
          .WithName("Game getting")
          .WithOpenApi();

        return web;
    }
}