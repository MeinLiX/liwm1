using Application.Futures.GamesFuture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class GameRouteHandlers
{
    internal static Delegate GamesGettingRouteHandler =>
        async (IMediator mediator, [FromQuery] int? id, int? start, int? count) =>
        {
            var request = new GamesGettingRequest
            {
                id = id,
                start = start,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };
}