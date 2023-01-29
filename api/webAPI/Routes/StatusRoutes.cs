using System;
using Application.Futures.StatusFuture.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class StatusRoutes
{
    public static WebApplication InitStatusRoutes(this WebApplication web)
        => web.InitPingRoute();


    public static WebApplication InitPingRoute(this WebApplication web)
    {
        web.MapGet("/ping", async (IMediator mediator) =>
        {
            var data = await mediator.Send(new PingQueryRequest());
            return Results.Json(data: data, statusCode: data.status_code);
        })
           .WithName("Ping")
           .WithOpenApi();

        return web;
    }
}

