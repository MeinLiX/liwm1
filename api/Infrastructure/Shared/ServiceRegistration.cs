using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Services.CloudinaryService;
using Shared.Services.PhotoService;
using Shared.Services.TokenService;

namespace Shared;

public static class ServiceRegistration
{
    public static IServiceCollection AddSharedInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        => services.AddJwtAuthenticationService(configuration)
                   .AddServices()
                   .AddCloudinary(configuration);

    public static IServiceCollection AddIdentityServices<T>(this IServiceCollection services) where T : DbContext
    {
        services.AddIdentityCore<AppUser>(options =>
                                         {
                                             options.Password.RequireDigit = true;
                                             options.Password.RequiredLength = 6;
                                             options.Password.RequireUppercase = true;
                                             options.Password.RequireLowercase = true;
                                             options.Password.RequireNonAlphanumeric = false;
                                         })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddEntityFrameworkStores<T>();

        return services;
    }

    private static IServiceCollection AddJwtAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                                                .GetBytes(configuration["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddScoped<ITokenService, TokenService>()
                   .AddTransient<ICloudinaryService, CloudinaryService>()
                   .AddTransient<IPhotoService, PhotoService>();

    private static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
}