using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TVMenukaart.Models;

namespace TVMenukaart.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            // var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            //
            // var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //
            // var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
            //
            // if (users == null)
            // {
            //     return;
            // }
            var userFaker = new UserFaker();
            var test = userFaker.Generate();

            var users = new UserFaker().Generate(10);


            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }

        public static async Task SeedRestaurants(TVMenukaartContext context)
        {
            if (await context.Restaurants.AnyAsync())
            {
                return;
            }

            var restaurantData = await File.ReadAllTextAsync("Data/RestaurantSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var restaurants = JsonSerializer.Deserialize<List<Restaurant>>(restaurantData, options);

            if (restaurants == null)
            {
                return;
            }

            foreach (var restaurant in restaurants)
            {
                var user = await context.Users.FindAsync(restaurant.AppUserId);
                restaurant.AppUser = user;
                await context.Restaurants.AddAsync(restaurant);
            }

            await context.SaveChangesAsync();
        }
    }
}
