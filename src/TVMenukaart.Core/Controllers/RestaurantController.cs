using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TVMenukaart.Data;
using TVMenukaart.DTO;
using TVMenukaart.Hubs;
using TVMenukaart.Models;

namespace TVMenukaart.Controllers
{
    [Authorize]
    public class RestaurantController : BaseApiController
    {
        private readonly ILogger<RestaurantController> _logger;
        private readonly TVMenukaartContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<RemoteMenuHub> _hubContext;

        public RestaurantController(ILogger<RestaurantController> logger, TVMenukaartContext context,
            UserManager<AppUser> userManager, IHubContext<RemoteMenuHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RestaurantDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRestaurants()
        {
            var user = await _userManager.GetUserAsync(User);
            var restaurants = await _context.Restaurants
                .Where(r => r.AppUser == user)
                .Select(m => new RestaurantDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    MenuCount = m.Menus.Count
                })
                .ToListAsync();

            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RestaurantDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRestaurant(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var restaurant = await _context.Restaurants
                .Where(r => r.AppUser == user)
                .Include(r => r.Menus)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (restaurant == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"Restaurant {restaurant.Name} with id {restaurant.Id} found. User {User.Identity.Name}");

            return Ok(new RestaurantDto
            {
                Id = restaurant.Id, Name = restaurant.Name, Menus = restaurant.Menus.Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name
                }).ToList()
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(RestaurantDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostRestaurant(Restaurant restaurant)
        {
            var newRestaurant = new Restaurant();

            newRestaurant.Name = restaurant.Name;
            newRestaurant.AppUser = await _userManager.GetUserAsync(User);
            _context.Restaurants.Add(newRestaurant);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Restaurant {newRestaurant.Name} with id {newRestaurant.Id} inserted. User {User.Identity.Name}");

            await _hubContext.Clients.All.SendAsync("ReceiveRestaurantsUpdate");

            return Ok(new RestaurantDto
            {
                Id = newRestaurant.Id,
                Name = newRestaurant.Name
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(RestaurantDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PutRestaurant(RestaurantDto restaurant)
        {
            var newRestaurant = await _context.Restaurants.FindAsync(restaurant.Id);

            if (newRestaurant == null)
            {
                return NotFound();
            }

            newRestaurant.Name = restaurant.Name;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Restaurant {restaurant.Name} with id {restaurant.Id} updated. User {User.Identity.Name}");

            await _hubContext.Clients.All.SendAsync("ReceiveRestaurantsUpdate");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Restaurant {restaurant.Name} with id {restaurant.Id} deleted. User {User.Identity.Name}");

            return NoContent();
        }
    }
}
