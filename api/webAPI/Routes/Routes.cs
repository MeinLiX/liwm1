using Application.Hubs;

namespace webAPI.Routes;

public static class Routes
{
    public static void InitRoutes(this WebApplication web)
    {
        web.InitAccountRoutes()
           .InitStatusRoutes()
           .InitPhotoRoutes()
           .InitGameRoutes()
           .InitHubs();
    }

    internal static WebApplication InitAccountRoutes(this WebApplication web)
    {
        web.MapPost("/account/login", AccountRouteHandlers.AccountLoginRouteHandler)
           .SetName(nameof(AccountRouteHandlers.AccountLoginRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For login account or create temporary anonymous account")
           .WithOpenApi();

        web.MapPost("/account/register", AccountRouteHandlers.AccountRegisterRouteHandler)
           .SetName(nameof(AccountRouteHandlers.AccountRegisterRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For register account")
           .WithOpenApi();

        web.MapPost("/account/logout", AccountRouteHandlers.LogoutRouteHandler)
           .SetName(nameof(AccountRouteHandlers.LogoutRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For logout from accounts\nIf account is anonymous then it will be deleted from the database")
           .WithOpenApi();

        return web;
    }

    internal static WebApplication InitStatusRoutes(this WebApplication web)
    {
        web.MapGet("/ping", StatusRouteHandlers.PingRouteHandler)
           .SetName(nameof(StatusRouteHandlers.PingRouteHandler))
           .SetTag(nameof(StatusRouteHandlers))
           .WithDescription("Check server status")
           .WithOpenApi();

        return web;
    }

    internal static WebApplication InitPhotoRoutes(this WebApplication web)
    {
        web.MapPost("/photos", PhotoRouteHandlers.GetPhotoRouteHandler)
           .SetName(nameof(PhotoRouteHandlers.GetPhotoRouteHandler))
           .SetTag(nameof(PhotoRouteHandlers))
           .WithDescription("For getting all photos from the database")
           .WithOpenApi();

        return web;
    }

    internal static WebApplication InitGameRoutes(this WebApplication web)
    {
        web.MapPost("/games", GameModeRouteHandlers.GameModesGettingRouteHandler)
           .SetName(nameof(GameModeRouteHandlers.GameModesGettingRouteHandler))
           .SetTag(nameof(GameModeRouteHandlers))
           .WithDescription("For getting all games from the database")
           .WithOpenApi();

        return web;
    }

    internal static WebApplication InitHubs(this WebApplication web)
    {
        web.MapHub<LobbyHub>("hubs/lobby");
        web.MapHub<RacingGameHub>("hubs/racing-game");

        return web;
    }

    #region RouteHandlerBuilder extension

    private static RouteHandlerBuilder SetName(this RouteHandlerBuilder routeBuilder, string name)
    => routeBuilder.WithName(name);

    private static RouteHandlerBuilder SetTag(this RouteHandlerBuilder routeBuilder, string tag)
    => routeBuilder.WithTags(tag.TagFormat());

    private static string TagFormat(this string value) => value.IndexOf("RouteHandlers") switch
    {
        int idx when idx > 0 => value.Remove(idx),
        _ => value,
    };

    #endregion RouteHandlerBuilder extension
}