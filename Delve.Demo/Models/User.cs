using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Delve.Models;

namespace Delve.Demo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }
    }

    public class UserQueryValidator : Delve.Models.Validation.AbstractQueryValidator<User>
    {
    }
}
