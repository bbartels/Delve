using Microsoft.EntityFrameworkCore;

using Delve.Demo.Models;

namespace Delve.Demo.Persistence
{
    public class UserManagerContext : DbContext
    {
        public UserManagerContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
