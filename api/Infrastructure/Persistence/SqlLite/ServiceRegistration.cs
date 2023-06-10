using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using SqlLite.Contexts;
using SqlLite.Repositories;

namespace SqlLite;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceSqlLiteInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContext<DataContext>(options => options.UseSqlite(configuration.GetConnectionString("SqlConnection")))
                   .AddScoped<IDataContext>(provider => provider.GetRequiredService<DataContext>())
                   .AddIdentityServices<DataContext>()
                   .AddRepositories();

    private static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddTransient<IUserRepository, UserRepository>()
                   .AddTransient<IPhotoRepository, PhotoRepository>()
                   .AddTransient<IGameModesRepository, GameModesRepository>()
                   .AddTransient<ILobbyRepository, LobbyRepository>()
                   .AddTransient<IRacingCarRepository, RacingCarRepository>()
                   .AddTransient<IGameRepository, GameRepository>();
}