using System.Reflection;
using webAPI.Routes;
using Application;
using PostgreSQL;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAplicationLoyaut();
builder.Services.AddPersistencePostgreSQLInfrastructureLoyaut();
builder.Services.AddSharedInfrastructureLoyaut();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.InitStatusRoutes();

app.Run();

