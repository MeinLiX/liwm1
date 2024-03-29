﻿using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using PostgreSQL.Contexts;
using PostgreSQL.Repositories;

namespace PostgreSQL;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistencePostgreSQLInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")))
                   .AddScoped<IDataContext>(provider => provider.GetRequiredService<DataContext>())
                   .AddIdentityServices<DataContext>()
                   .AddRepositories();

    private static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddTransient<IUserRepository, UserRepository>()
                   .AddTransient<IPhotoRepository, PhotoRepository>()
                   .AddTransient<IGameModesRepository, GameModesRepository>()
                   .AddTransient<ILobbyRepository, LobbyRepository>()
                   .AddTransient<IRacingCarRepository, RacingCarRepository>()
                   .AddTransient<IGameRepository, GameRepository>()
                   .AddTransient<ILevelsRepository, LevelsRepository>();
}