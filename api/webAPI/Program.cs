using System.Reflection;
using webAPI.Routes;
using Application;
using PostgreSQL;
using Shared;
using SqlLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAplicationLayer();
// builder.Services.AddPersistencePostgreSQLInfrastructureLayer();
builder.Services.AddPersistenceSqlLiteInfrastructureLayer(builder.Configuration);
builder.Services.AddSharedInfrastructureLayer();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.InitStatusRoutes();

app.Run();

