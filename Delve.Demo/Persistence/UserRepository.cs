using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Delve.Demo.Models;

namespace Delve.Demo.Persistence
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserManagerContext UserManagerContext { get { return Context as UserManagerContext; } }
        public UserRepository(DbContext context) : base(context) { }

        public async Task<int> GetIdAsync(string username)
        {
            return await UserManagerContext.Users.Where(u => u.FirstName == username).Select(u => u.Id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesAsync(User user)
        {
            return await UserManagerContext.Entry(user).Collection(u => u.UserRoles)
                .Query().Select(ur => ur.Role).ToListAsync();
        }
    }
}
