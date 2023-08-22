using Application.Futures.GameModesFuture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class GameModeRouteHandlers
{
    internal static Delegate GameModesGettingRouteHandler =>
        async (IMediator mediator, [FromQuery] int? id, string? name, int? start, int? count) =>
        {
            var request = new GameModesGettingRequest
            {
                id = id,
                start = start,
                name = name,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };
}