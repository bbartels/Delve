using System;
using System.Collections.Generic;

namespace Delve.Demo.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public IEnumerable<UserRoleDto> UserRoles { get; set; }
    }
}
