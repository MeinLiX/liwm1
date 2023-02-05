using Application.Futures.PhotoFuture.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Routes;

public static class Routes
{
    public static WebApplication InitPhotoRoutes(this WebApplication web)
        => web.InitPhotoRoute();

    private static WebApplication InitPhotoRoute(this WebApplication web)
    {
        web.MapPost("/photos", async (IMediator mediator, [FromQuery] int? id, int? start, int? count) =>
        {
            var request = new PhotoRequest
            {
                id = id,
                start = start,
                count = count
            };
            var data = await mediator.Send(request);
            return Results.Json(data: data, statusCode: data.status_code);
        })
          .WithName("Photo getting")
          .WithOpenApi();

        return web;
    }
}