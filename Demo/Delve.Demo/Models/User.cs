using System;
using System.Collections.Generic;
using System.Linq;

using Delve.Models.Validation;

namespace Delve.Demo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }

        public User() { }

        public User(string firstName, string lastName, DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }

    public class UserQueryValidator : AbstractQueryValidator<User>
    {
        public UserQueryValidator()
        {
            //Allows clients to select Id
            CanSelect("Id", x => x.Id);

            //Allows clients to select virtual property Name
            CanSelect("Name", x => x.FirstName + " " + x.LastName);

            //Allows clients to order by virtual property Name
            CanOrder("Name", x => x.LastName + " " + x.FirstName);

            //Allows clients to order by virtual property Age
            CanSelect("Age", x => Math.Round((DateTime.Now - x.DateOfBirth).TotalDays / 365, 2));
            CanOrder("Age", x => Math.Round((DateTime.Now - x.DateOfBirth).TotalDays / 365, 2));

            //Allows clients to filter on virtual property Name
            CanFilter("Name", x => x.FirstName + " " + x.LastName);
            CanFilter("Id", x => x.Id);

            CanSelect("RoleId", x => x.UserRoles.Select(ur => ur.RoleId));

            //Allows clients to expand on UserRoles (EF Core Include)
            //WARNING: Can lead to bad performance if used incorrectly.
            CanExpand("UserRoles", x => x.UserRoles);
        }
    }
}
