using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TVMenukaart.Data;
using TVMenukaart.Extensions;
using TVMenukaart.Helpers;
using TVMenukaart.Hubs;
using TVMenukaart.Interfaces;
using TVMenukaart.Models;
using TVMenukaart.Services;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("JWT Token", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header
    });
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add(
        new ProducesResponseTypeAttribute(typeof(ProblemDetails), (int)HttpStatusCode.NotFound));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails),
        (int)HttpStatusCode.InternalServerError));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ValidationProblemDetails),
        (int)HttpStatusCode.BadRequest));
});
builder.Services.AddSignalR();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);
builder.Services.AddScoped<IBackgroundService, BackgroundImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRouting();
app.UseCors();
app.Use(async (context, next) =>
{
    Console.WriteLine($"[Before Auth] {context.Request.Path} => Authenticated: {context.User?.Identity?.IsAuthenticated}");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine("Auth Header: " + context.Request.Headers["Authorization"]);
    Console.WriteLine($"[After Auth] {context.Request.Path} => Authenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"Name: {context.User?.Identity?.Name}");
    foreach (var claim in context.User.Claims)
    {
        Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
    }

    await next();
});

app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<TVMenukaartContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager);
    await Seed.SeedRestaurants(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.MapHub<RemoteMenuHub>("menuHub");

app.Run();

public partial class Program;
