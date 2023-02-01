using System.Reflection;
using webAPI.Routes;
using Application;
using PostgreSQL;
using Shared;
using SqlLite;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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