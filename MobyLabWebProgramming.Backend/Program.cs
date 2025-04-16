using System.Text.Json.Serialization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Database;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Adaugă serviciile aplicației + suport pentru enumuri ca string în JSON
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.AddCorsConfiguration()
    .AddRepository()
    .AddAuthorizationWithSwagger("MobyLab Web App")
    .AddServices()
    .UseLogger()
    .AddWorkers()
    .AddApi();

var app = builder.Build();

app.ConfigureApplication();

// Seed pentru contul admin (o singură dată la pornire)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WebAppDatabaseContext>();
    await UserSeeder.SeedAsync(dbContext);
}

app.Run();
