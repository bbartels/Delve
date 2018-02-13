using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Delve.Demo.Models;

namespace Delve.Demo.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
