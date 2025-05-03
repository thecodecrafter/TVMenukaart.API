using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RemoteMenu.Data;
using RemoteMenu.DTO;
using RemoteMenu.Hubs;
using RemoteMenu.Interfaces;
using RemoteMenu.Models;

namespace RemoteMenu.Controllers
{
    [Authorize]
    public class MenuController : BaseApiController
    {
        private readonly RemoteMenuContext _context;
        private readonly IHubContext<RemoteMenuHub> _hubContext;
        private readonly IBackgroundService _backgroundImageService;
        private readonly UserManager<AppUser> _userManager;

        public MenuController(RemoteMenuContext context, IBackgroundService backgroundImageService,
            IHubContext<RemoteMenuHub> hubContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _backgroundImageService = backgroundImageService;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMenus()
        {
            var user = await _userManager.GetUserAsync(User);

            var menus = await _context.Menus.Where(m => m.AppUser == user)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    PublicUrl = m.PublicUrl
                }).ToListAsync();

            var menuDtos = menus.Select((menu) => new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                PublicUrl = menu.PublicUrl
            });

            return Ok(menuDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MenuDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMenu(int id)
        {
            var menu = await _context.Menus
                .FindAsync(id);
            
            if (menu == null)
            {
                return NotFound();
            }
            
            // retrieve the MenuSections and MenuItems
            await _context.Entry(menu).Collection(m => m.MenuSections)
                .Query()
                .Include(m => m.MenuItems)
                .LoadAsync();

            var menuDto = new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                PublicUrl = menu.PublicUrl,
                MenuSections = menu.MenuSections.Select(m => new MenuSectionDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    MenuItems = m.MenuItems.Select(i => new MenuItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price
                    }).ToList()
                }).ToList()
            };

            return Ok(menuDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MenuDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostMenu(string name, int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            if (restaurant == null)
            {
                return NotFound();
            }

            // check if current user is owner of restaurant
            var appUser = await _userManager.GetUserAsync(User);
            if (restaurant.AppUser != appUser)
            {
                return Unauthorized();
            }

            var menu = new Menu();
            menu.Name = name;
            menu.PublicUrl = Guid.NewGuid().ToString("N");
            menu.Restaurant = restaurant;
            menu.AppUser = appUser;

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            return Ok(new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                PublicUrl = menu.PublicUrl
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> PutMenu(int id, MenuDto menu)
        {
            if (id != menu.Id)
            {
                return BadRequest();
            }

            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound();
            }

            existingMenu.Name = menu.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");
            return NoContent();
        }

        [HttpPost("{id}/board-photo")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<BoardPhoto>> PostMenuBoardPhoto(int id, IFormFile file)
        {
            // get the menu
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            // get user first to correctly upload the background to the correct user
            var result = await _backgroundImageService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var boardPhoto = new BoardPhoto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            menu.BoardPhoto = boardPhoto;

            if (await _context.SaveChangesAsync() > 0)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");
                return boardPhoto;
            }

            return NoContent();
        }
    }
}
