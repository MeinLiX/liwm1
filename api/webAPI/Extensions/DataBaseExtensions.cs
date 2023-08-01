using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace webAPI.Extensions;

public static class DataBaseExtensions
{
    public static async Task MigrateDataBaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<IDataContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await roleManager.CreateRolesAsync(context);

        await context.SyncPhotosAsync(services.GetRequiredService<IPhotoService>());
        await context.SyncGamesAsync(services.GetRequiredService<IPhotoService>());
    }

    private static async Task CreateRolesAsync(this RoleManager<AppRole> roleManager, IDataContext context)
    {
        var roles = new AppRole[]
        {
            new AppRole { Name = "Gamer" },
            new AppRole { Name = "Anonymous" },
            new AppRole { Name = "Admin" }
        };

        foreach (var role in context.Roles)
        {
            if (role != null && !roles.Contains(role))
            {
                await roleManager.DeleteAsync(role);
            }
        }

        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role.Name) is null)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SyncPhotosAsync(this IDataContext context, IPhotoService photoService)
    {
        var photos = await photoService.GetUsersPhotosAsync();
        await context.Photos.AddRangeAsync(photos.Where(photo => context.Photos.All(p => p.Url != photo))
                                                 .Select(p => new Photo { Url = p }));
        context.Photos.RemoveRange(context.Photos.Where(p => !photos.Contains(p.Url)));
        await context.SaveChangesAsync();
    }

    private static async Task SyncGamesAsync(this IDataContext context, IPhotoService photoService)
    {
        var gameModes = new GameMode[]
        {
            new GameMode
            {
                Name = "Racing",
                PreviewUrl = await photoService.GetGamePhotoAsync("Racing")
            },
            new GameMode
            {
                Name = "Words Battle",
                PreviewUrl =  await photoService.GetGamePhotoAsync("WordsBattle")
            }
        };

        await context.GameModes.AddRangeAsync(gameModes.Where(g => context.GameModes.All(cg => cg.Name != g.Name)));
        await context.SaveChangesAsync();
    }
}