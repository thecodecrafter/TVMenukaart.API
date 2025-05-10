using Microsoft.AspNetCore.Identity;

namespace TVMenukaart.Models
{
    public class AppUser : IdentityUser<int>
    {
        public ICollection<Restaurant> Restaurant { get; set; } = [];
        public ICollection<AppUserRole> UserRoles { get; set; } = [];
        public ICollection<Menu> Menus { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
