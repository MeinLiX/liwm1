using webAPI.Routes;

namespace webAPI.Extensions;

public static class RoutesInitializationExtensions
{
    public static WebApplication InitRoutes(this WebApplication web)
        => web.InitStatusRoutes()
              .InitAccountRoutes()
              .InitPhotoRoutes();
}