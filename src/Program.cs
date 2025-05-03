using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RemoteMenu.Data;
using RemoteMenu.Extensions;
using RemoteMenu.Helpers;
using RemoteMenu.Hubs;
using RemoteMenu.Interfaces;
using RemoteMenu.Models;
using RemoteMenu.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("JWT Token", new OpenApiSecurityScheme()
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
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<RemoteMenuContext>();
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
