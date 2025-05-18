using Bogus;
using Microsoft.AspNetCore.Identity;
using TVMenukaart.Data;
using TVMenukaart.Models;

namespace TVMenukaart.IntegrationTests.TestData
{
    public class TestDataSeeder
    {
        private readonly TVMenukaartContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly Faker _faker;

        private readonly List<AppUser> _users = new();
        private readonly List<Restaurant> _restaurants = new();
        private readonly List<Models.Menu> _menus = new();

        public TestDataSeeder(TVMenukaartContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _faker = new Faker();
        }

        public async Task SeedAsync()
        {
            await SeedUsersAsync();
            await SeedRestaurantsAsync();
            await SeedMenusAsync();
            
            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            var userFaker = new Faker<AppUser>()
                .CustomInstantiator(f => new AppUser()
                {
                    UserName = f.Person.UserName,
                    Email = f.Person.Email,
                    PhoneNumber = f.Phone.PhoneNumber()
                });
            
            _users.AddRange(userFaker.Generate(10));
            
            foreach (var user in _users)
            {
                await _userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }

        private async Task SeedRestaurantsAsync()
        {
            var random = new Random();
            var restaurantFaker = new Faker<Restaurant>()
                .CustomInstantiator(f => new Restaurant()
                {
                    Name = f.Company.CompanyName(),
                    AppUser = _users[random.Next(_users.Count)]
                });
            
            _restaurants.AddRange(restaurantFaker.Generate(10));
            await _context.Restaurants.AddRangeAsync(_restaurants);
        }

        private async Task SeedMenusAsync()
        {
            var random = new Random();
            var menuFaker = new Faker<Models.Menu>()
                .CustomInstantiator(f => new Models.Menu()
                {
                    Name = f.Commerce.ProductName(),
                    Restaurant = _restaurants[random.Next(_restaurants.Count)],
                    AppUser = _users[random.Next(_users.Count)]
                });
            
            _menus.AddRange(menuFaker.Generate(10));
            await _context.Menus.AddRangeAsync(_menus);
        }
        
        public Restaurant GetTestRestaurant(int index = 0) => _restaurants[index];
        public Restaurant GetTestRestaurantByUser(int appUserId) => _restaurants.Find(r => r.AppUser.Id == appUserId);
        public AppUser GetTestUser(int index = 0) => _users[index];
        public Models.Menu GetTestMenu(int index = 0) => _menus[index];
    }
}
