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
        await roleManager.CreateRolesAsync();
    }

    private static async Task CreateRolesAsync(this RoleManager<AppRole> roleManager)
    {
        var roles = new AppRole[]
        {
            new AppRole { Name = "Gamer" },
            new AppRole { Name = "Admin" }
        };

        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role.Name) is null)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}