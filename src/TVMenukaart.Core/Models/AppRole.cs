using Microsoft.AspNetCore.Identity;

namespace TVMenukaart.Models
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = [];
    }
}
