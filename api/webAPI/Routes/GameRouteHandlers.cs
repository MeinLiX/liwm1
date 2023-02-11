using Application.Futures.GamesFuture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class GameRouteHandlers
{
    //m1adow??? change name
    internal static Delegate GameRouteHandler =>
        async (IMediator mediator, [FromQuery] int? id, int? start, int? count) =>
        {
            var request = new GameRequest
            {
                id = id,
                start = start,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };
}