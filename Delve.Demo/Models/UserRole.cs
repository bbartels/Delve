using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delve.Demo.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public User User { get; set; }
    }
}
