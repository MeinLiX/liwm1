using System.Reflection;
using webAPI.Routes;
using Application;
using PostgreSQL;
using Shared;
using SqlLite;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearerAuth"
                }
            },
            new string[]{ }
        }
    });
});

builder.Services.AddAplicationLayer();
// builder.Services.AddPersistencePostgreSQLInfrastructureLayer();
builder.Services.AddPersistenceSqlLiteInfrastructureLayer(builder.Configuration);
builder.Services.AddSharedInfrastructureLayer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.InitStatusRoutes();
app.InitAccountRoutes();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<IDataContext>();
await context.Database.MigrateAsync();

app.Run();