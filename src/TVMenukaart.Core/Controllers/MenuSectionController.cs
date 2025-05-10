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
    public class MenuSectionController : BaseApiController
    {
        private readonly ILogger<MenuSectionController> _logger;
        private readonly TVMenukaartContext _context;
        private readonly IHubContext<RemoteMenuHub> _hubContext;

        public MenuSectionController(
            TVMenukaartContext context,
            IHubContext<RemoteMenuHub> hubContext,
            ILogger<MenuSectionController> logger
        )
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet("admin/{menuId}")]
        [ProducesResponseType(typeof(IEnumerable<MenuSectionDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMenuSections(int menuId)
        {
            var menu = await _context.Menus
                .Include(m => m.MenuSections)
                .ThenInclude(i => i.MenuItems)
                .FirstOrDefaultAsync(i => i.Id == menuId);

            if (menu == null)
            {
                return NotFound();
            }

            return Ok(menu.MenuSections);
        }

        // [HttpGet("{uid}")]
        // [AllowAnonymous]
        // public async Task<ActionResult<IEnumerable<MenuSectionDto>>> GetMenuSectionsByUid(string uid)
        // {
        //     var menu = await _context.Menus
        //         .Include(m => m.MenuSections)
        //         .ThenInclude(m => m.MenuItems)
        //         .FirstOrDefaultAsync(i => i.PublicUrl == uid);
        //
        //     if (menu == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return Ok(menu.MenuSections);
        // }

        [HttpPost]
        [ProducesResponseType(typeof(MenuSectionDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostMenuSection(int menuId, string sectionName)
        {
            var menu = await _context.Menus.FindAsync(menuId);

            if (menu == null)
            {
                return NotFound();
            }

            var menuSection = new MenuSection(sectionName);
            menu.MenuSections.Add(menuSection);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            var menuSectionDto = new MenuSectionDto();
            menuSectionDto.Name = menuSection.Name;
            menuSectionDto.Id = menuSection.Id;

            return Ok(menuSectionDto);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(MenuSectionDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PutMenuSection(int menuId, int menuSectionId, string sectionName)
        {
            var menu = await _context.Menus
                .Include(i => i.MenuSections)
                .FirstOrDefaultAsync(i => i.Id == menuId);

            if (menu == null)
            {
                return NotFound();
            }

            var menuSection = menu.MenuSections.FirstOrDefault(i => i.Id == menuSectionId);

            if (menuSection == null)
            {
                return BadRequest();
            }

            menuSection.Name = sectionName;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate");

            var menuSectionDto = new MenuSectionDto();
            menuSectionDto.Name = menuSection.Name;
            menuSectionDto.Id = menuSection.Id;

            return Ok(menuSectionDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuSection(int id)
        {
            var menuSection = await _context.MenuSections.FindAsync(id);
            if (menuSection == null)
            {
                return NotFound();
            }

            _context.MenuSections.Remove(menuSection);
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
