using Microsoft.AspNetCore.Identity;

namespace RemoteMenu.Models
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = [];
    }
}
