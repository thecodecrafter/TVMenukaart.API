using Microsoft.EntityFrameworkCore;
using TVMenukaart.Data;
using TVMenukaart.Interfaces;
using TVMenukaart.Services;

namespace TVMenukaart.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<TVMenukaartContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
            );
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins("http://localhost:5174", "http://localhost:5173", "https://192.168.0.82",
                            "https://tvmenukaartclient.azurewebsites.net", "http://172.20.10.13:8081",
                            "https://192.168.0.82:8081", "https://10.0.2.2:7253")
                        // .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod();
                });
            });
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
