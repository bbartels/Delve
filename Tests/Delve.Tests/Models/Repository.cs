using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delve.Tests.Models
{
    internal static class Repository
    {
        private static readonly List<User> _users;

        static Repository()
        {
            var users = new List<User>
            {
                new User(1, "Veronica", "Fear", DateTime.Parse("23/01/1946 21:01:40")),
                new User(2, "Isabelle", "Bishop", DateTime.Parse("16/04/1968 18:00:00")),
                new User(3, "Wilf", "Crawford", DateTime.Parse("14/03/1975 16:56:40")),
                new User(4, "Eliza", "Bentley", DateTime.Parse("31/03/1996 19:41:40")),
                new User(5, "Steve", "Grenville", DateTime.Parse("19/08/1948 10:15:00")),
                new User(6, "Lainey", "Harmon", DateTime.Parse("17/04/1923 11:40:00")),
                new User(7, "Rosaline", "Kelsey", DateTime.Parse("25/10/1926 01:31:40")),
                new User(8, "Alvena", "Albinson", DateTime.Parse("26/01/1998 22:43:20")),
                new User(9, "Ivan", "Keighley", DateTime.Parse("03/06/1945 08:31:40")),
                new User(10, "Gordie", "Jack", DateTime.Parse("12/08/1930 22:46:40")),
                new User(11, "Delight", "Burnham", DateTime.Parse("15/11/1968 15:18:20")),
                new User(12, "Aleta", "Huddleson", DateTime.Parse("20/03/1976 02:18:20")),
                new User(13, "Hedley", "Lund", DateTime.Parse("25/12/1940 15:13:20")),
                new User(14, "Jeni", "Bristow", DateTime.Parse("18/01/1935 07:33:20")),
                new User(15, "Phyllis", "Waters", DateTime.Parse("17/03/1962 22:46:40")),
                new User(16, "Ryley", "Tuff", DateTime.Parse("30/03/1989 23:51:40")),
                new User(17, "Sharise", "Garrard", DateTime.Parse("18/06/1978 02:31:40")),
                new User(18, "Krysten", "Cannon", DateTime.Parse("04/02/1980 12:11:40")),
                new User(19, "Fulke", "Bullock", DateTime.Parse("02/03/2006 09:53:20")),
                new User(20, "Brand", "Chambers", DateTime.Parse("22/10/1992 18:51:40"))
            };

            var roles = new List<Role>
            {
                new Role { Id = 1, Description = "Admin", Name = "admin" },
                new Role { Id = 2, Description = "Moderator", Name = "Moderator" },
                new Role { Id = 3, Description = "Staff", Name = "Staff" },
                new Role { Id = 4, Description = "Infomation Techonogies", Name = "IT" },
                new Role { Id = 5, Description = "Finance", Name = "finance" },
                new Role { Id = 6, Description = "Human Resources", Name = "HR" }
            };

            var userRoles = new List<UserRole>
            {
                new UserRole(1, 1, 4),  new UserRole(2, 20, 1), new UserRole(3, 12, 5),
                new UserRole(4, 17, 1), new UserRole(5, 8, 3),  new UserRole(6, 3, 3),
                new UserRole(7, 15, 1), new UserRole(8, 12, 6), new UserRole(9, 12, 4),
                new UserRole(10, 11, 1), new UserRole(11, 4, 3),  new UserRole(12, 16, 2),
                new UserRole(13, 10, 4), new UserRole(14, 10, 6), new UserRole(15, 14, 4),
                new UserRole(16, 2, 2),  new UserRole(17, 18, 6), new UserRole(18, 5, 5),
                new UserRole(19, 17, 6), new UserRole(20, 9, 1),  new UserRole(21, 9, 4)
            };


            foreach (var userRole in userRoles)
            {
                foreach (var role in roles)
                {
                    if (role.Id == userRole.RoleId)
                    {
                        userRole.Role = role;
                    }
                }
            }

            foreach (var user in users)
            {
                var list = new List<UserRole>();
                foreach (var userRole in userRoles)
                {
                    if (user.Id == userRole.UserId)
                    {
                        list.Add(userRole);
                    }
                }

                user.UserRoles = list;
            }

            _users = users;
        }

        public static List<User> GetUsers()
        {
            return new List<User>(_users);
        }
    }
}
