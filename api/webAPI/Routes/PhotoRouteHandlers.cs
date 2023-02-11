using Application.Futures.PhotoFuture.Queries;
using Application.Futures.StatusFuture.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class PhotoRouteHandlers
{
    internal static Delegate GetPhotoRouteHandler =>
        async (IMediator mediator, [FromQuery] int? id, int? start, int? count) =>
        {
            var request = new PhotoRequest
            {
                id = id,
                start = start,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        };
}