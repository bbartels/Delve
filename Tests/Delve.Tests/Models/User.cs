using System;
using System.Collections.Generic;

namespace Delve.Tests.Models
{
    internal class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Test Test { get; set; }

        public IEnumerable<UserRole> UserRoles { get; set; }

        public User(int id, string firstName, string lastName, DateTime dateOfBirth)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }

    internal class Test
    {
        public string TestString { get; set; }
    }
}
