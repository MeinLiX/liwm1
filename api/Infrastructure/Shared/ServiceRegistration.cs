using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services.TokenService;

namespace Shared;

public static class ServiceRegistration
{
    public static IServiceCollection AddSharedInfrastructureLayer(this IServiceCollection services)
        => services.AddScoped<ITokenService, TokenService>();
}

