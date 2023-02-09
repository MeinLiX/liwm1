using Application.Futures.AccountFuture.Commands;
using Application.Futures.AccountFuture.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class AccountRoutes
{
    public static WebApplication InitAccountRoutes(this WebApplication web)
        => web.InitAccountLoginRoute()
              .InitAccountRegisterRoute()
              .InitAnonymousLoginRoute();

    private static WebApplication InitAccountLoginRoute(this WebApplication web)
    {
        web.MapPost("/account/login", async (IMediator mediator, [FromBody] AccountLoginRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        })
           .WithName("Account login")
           .WithOpenApi();

        return web;
    }

    private static WebApplication InitAccountRegisterRoute(this WebApplication web)
    {
        web.MapPost("/account/register", async (IMediator mediator, [FromBody] AccountRegisterRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        })
          .WithName("Account register")
          .WithOpenApi();

        return web;
    }

    private static WebApplication InitAnonymousLoginRoute(this WebApplication web)
    {
        web.MapPost("/account/anonymous", async (IMediator mediator, [FromBody] AnonymousUserLoginRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        })
          .WithName("Anonymous login")
          .WithOpenApi();

        return web;
    }
}