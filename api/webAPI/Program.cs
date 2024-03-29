﻿using Application;
using Shared;
using PostgreSQL;
using Microsoft.OpenApi.Models;
using webAPI.Middleware;
using webAPI.Extensions;
using webAPI.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAppConfiguration(builder.Environment, args);
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSignalR();
builder.Services.AddAplicationLayer();
builder.Services.AddPersistencePostgreSQLInfrastructureLayer(builder.Configuration);
builder.Services.AddSharedInfrastructureLayer(builder.Configuration);

builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithOrigins(builder.Configuration["OriginsUrl"]));

app.UseAuthentication();
app.UseAuthorization();

app.InitRoutes();

await app.Services.MigrateDataBaseAsync();

app.Run();