using Application.Futures.StatusFuture.Queries;
using MediatR;

namespace webAPI.Routes;

public static class StatusRouteHandlers
{
    internal static Delegate PingRouteHandler =>
        async (IMediator mediator) =>
        {
            var data = await mediator.Send(new PingQueryRequest());
            return Results.Json(data: data, statusCode: data.status_code);
        };
}

