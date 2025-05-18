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
    app.ApplyMigrations();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.MapHub<RemoteMenuHub>("menuHub");

app.Run();

public partial class Program;
