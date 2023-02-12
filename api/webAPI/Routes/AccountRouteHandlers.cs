using Application.Futures.AccountFuture.Commands;
using Application.Futures.AccountFuture.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class AccountRouteHandlers
{
    internal static Delegate AccountLoginRouteHandler =>
        async (IMediator mediator, [FromBody] AccountLoginRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };

    internal static Delegate AccountRegisterRouteHandler =>
        async (IMediator mediator, [FromBody] AccountRegisterRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };

    internal static Delegate LogoutRouteHandler =>
        async (IMediator mediator, [FromBody] UserLogoutRequest request) =>
        {
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };
}