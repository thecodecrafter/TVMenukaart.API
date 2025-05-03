using Microsoft.EntityFrameworkCore;
using RemoteMenu.Data;
using RemoteMenu.Interfaces;
using RemoteMenu.Services;

namespace RemoteMenu.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<RemoteMenuContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection"))
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
