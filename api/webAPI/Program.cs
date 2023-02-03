using webAPI.Routes;
using Application;
using PostgreSQL;
using Shared;
using SqlLite;
using Microsoft.OpenApi.Models;
using webAPI.Middleware;
using webAPI.Extensions;

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

builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

app.InitStatusRoutes();
app.InitAccountRoutes();

await app.Services.MigrateDataBaseAsync();

app.Run();