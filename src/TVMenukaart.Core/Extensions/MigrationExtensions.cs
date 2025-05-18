using Microsoft.EntityFrameworkCore;
using TVMenukaart.Data;

namespace TVMenukaart.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<TVMenukaartContext>();
            
            await dbContext.Database.MigrateAsync();
            // var userManager = services.GetRequiredService<UserManager<AppUser>>();
            
            
                
            // await Seed.SeedUsers(userManager);
            // await Seed.SeedRestaurants(dbContext);
        }
    }
}
