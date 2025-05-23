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
        private readonly List<Menu> _menus = new();
        private readonly List<MenuItem> _menuItems = new();
        private readonly List<MenuSection> _menuSections = new();

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
            await SeedMenuSectionsAsync();
            await SeedMenuItemsAsync();

            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            var userFaker = new Faker<AppUser>()
                .CustomInstantiator(f => new AppUser
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
            var restaurantFaker = new Faker<Models.Restaurant>()
                .CustomInstantiator(f => new Models.Restaurant
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
                .CustomInstantiator(f => new Models.Menu
                {
                    Name = f.Commerce.ProductName(),
                    Restaurant = _restaurants[random.Next(_restaurants.Count)],
                    AppUser = _users[random.Next(_users.Count)]
                });

            _menus.AddRange(menuFaker.Generate(10));
            await _context.Menus.AddRangeAsync(_menus);
        }

        private async Task SeedMenuItemsAsync()
        {
            var random = new Random();
            var menuItemFaker = new Faker<MenuItem>()
                .CustomInstantiator(f => new MenuItem
                {
                    Name = f.Commerce.ProductName(),
                    Description = f.Lorem.Sentence(6),
                    Price = f.Finance.Amount(1, 100),
                    MenuSection = _menuSections[random.Next(_menuSections.Count)]
                });

            _menuItems.AddRange(menuItemFaker.Generate(10));
            await _context.MenuItems.AddRangeAsync(_menuItems);
        }

        private async Task SeedMenuSectionsAsync()
        {
            var random = new Random();
            var menuSectionFaker = new Faker<Models.MenuSection>()
                .CustomInstantiator(f => new Models.MenuSection(f.Commerce.ProductName())
                {
                    Menu = _menus[random.Next(_menus.Count)]
                    // MenuItems = _menuItems.GetRange(0, 5)
                });

            _menuSections.AddRange(menuSectionFaker.Generate(10));
            await _context.MenuSections.AddRangeAsync(_menuSections);
        }

        public Restaurant GetTestRestaurant(int index = 0)
        {
            return _restaurants[index];
        }

        public Restaurant GetTestRestaurantByUser(int appUserId)
        {
            return _restaurants.Find(r => r.AppUser.Id == appUserId);
        }

        public AppUser GetTestUser(int index = 0)
        {
            return _users[index];
        }

        public Models.Menu GetTestMenu(int index = 0)
        {
            return _menus[index];
        }

        public MenuItem GetTestMenuItem(int index = 0)
        {
            return _menuItems[index];
        }

        public MenuSection GetTestMenuSection(int index = 0)
        {
            return _menuSections[index];
        }
    }
}
