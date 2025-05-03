using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RemoteMenu.Models;

namespace RemoteMenu.Data
{
    public class RemoteMenuContext(DbContextOptions options)
        : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
            IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
    {
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<BoardPhoto> BoardPhotos { get; set; }
        public DbSet<MenuSection> MenuSections { get; set; }
        public DbSet<DeviceCode> DeviceCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<DeviceCode>()
                .HasNoKey()
                .HasKey(d => d.PollingToken);
        }
    }
}
