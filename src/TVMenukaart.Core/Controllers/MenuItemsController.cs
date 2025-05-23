using System.Net;
using Microsoft.AspNetCore.Authorization;
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
    public class MenuItemsController : BaseApiController
    {
        private readonly TVMenukaartContext _context;
        private readonly IHubContext<RemoteMenuHub> _hubContext;

        public MenuItemsController(TVMenukaartContext context, IHubContext<RemoteMenuHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("menuSection/{menuSectionId}")]
        [ProducesResponseType(typeof(IEnumerable<MenuItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMenuItems(int menuSectionId)
        {
            var menuSection = await _context
                .MenuSections.Include(m => m.MenuItems)
                .FirstOrDefaultAsync(i => i.Id == menuSectionId);

            if (menuSection == null)
            {
                return NotFound();
            }

            var menuItemDtos = menuSection.MenuItems.Select(menuItem => new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price
            }).ToList();

            return Ok(menuItemDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MenuItemDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                // .Include(c => c.Category)
                .SingleOrDefaultAsync(x => x.Id == id);
            // .FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price
            };

            return Ok(menuItemDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MenuItemDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostMenuItem(MenuItemDto menuItemDto)
        {
            var menuSection = await _context.MenuSections.FindAsync(menuItemDto.MenuSectionId);

            var menuItem = new MenuItem
            {
                Description = menuItemDto.Description,
                Name = menuItemDto.Name,
                Price = menuItemDto.Price
            };

            menuSection.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            return Ok(menuItemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuItem(int id, MenuItemDto menuItemDto)
        {
            if (id != menuItemDto.Id)
            {
                return BadRequest();
            }

            var existingMenuItem = await _context.MenuItems.FindAsync(id);
            if (existingMenuItem == null)
            {
                return NotFound();
            }

            existingMenuItem.Name = menuItemDto.Name;
            existingMenuItem.Price = menuItemDto.Price;
            existingMenuItem.Description = menuItemDto.Description;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }
}
