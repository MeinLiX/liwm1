namespace webAPI.Routes;

public static class Routes
{
    public static void InitRoutes(this WebApplication web)
    {
        web.InitAccountRoutes()
           .InitStatusRoutes()
           .InitPhotoRoutes()
           .InitGameRoutes();
    }

    //m1adown add descriptions
    internal static WebApplication InitAccountRoutes(this WebApplication web)
    {
        web.MapPost("/account/login", AccountRouteHandlers.AccountLoginRouteHandler)
           .SetName(nameof(AccountRouteHandlers.AccountLoginRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For login account")
           .WithOpenApi();

        web.MapPost("/account/register", AccountRouteHandlers.AccountRegisterRouteHandler)
           .SetName(nameof(AccountRouteHandlers.AccountRegisterRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For register account")
           .WithOpenApi();

        web.MapPost("/account/logout", AccountRouteHandlers.AnonymousLogoutRouteHandler)
           .SetName(nameof(AccountRouteHandlers.AnonymousLogoutRouteHandler))
           .SetTag(nameof(AccountRouteHandlers))
           .WithDescription("For logout anonymous account")
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
           .WithDescription("Check server status")
           .WithOpenApi();

        return web;
    }

    internal static WebApplication InitGameRoutes(this WebApplication web)
    {
        web.MapPost("/games", GameRouteHandlers.GameRouteHandler)
           .SetName(nameof(GameRouteHandlers.GameRouteHandler))
           .SetTag(nameof(GameRouteHandlers))
           .WithDescription("???   change name!!!")
           .WithOpenApi();

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

