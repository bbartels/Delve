using System.Collections.Generic;

namespace Delve.Demo.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}
