using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delve.Demo.Models;
using Microsoft.EntityFrameworkCore;

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
