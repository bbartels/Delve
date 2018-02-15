using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Delve.Demo.Persistence;
using Delve.Demo.Models;
using Delve.Models.Validation;
using Delve.AspNetCore;

namespace Delve.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Delve to this Mvc Project
            services.AddMvc().AddDelve(options => options.IgnoreNullOnSerilazation=false);

            const string connection = @"Server=(localdb)\mssqllocaldb;Database=DelveDemo;Trusted_Connection=True;";
            services.AddDbContext<UserManagerContext>(options => options.UseSqlServer(connection));

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Add the QueryValidator for User
            services.AddTransient<IQueryValidator<User>, UserQueryValidator>();

            var context = services.BuildServiceProvider().GetRequiredService<UserManagerContext>();

            if (context.Database.EnsureCreated())
            {
                context.EnsureSeeded();
            }

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    public static class ConxtexExtensions
    {
        public static void EnsureSeeded(this UserManagerContext context)
        {
            if (!context.Database.GetPendingMigrations().Any())
            {
                var users = new List<User>
                {
                    new User("Veronica", "Fear", DateTime.Parse("23/01/1946 21:01:40")),
                    new User("Isabelle", "Bishop", DateTime.Parse("16/04/1968 18:00:00")),
                    new User("Wilf", "Crawford", DateTime.Parse("14/03/1975 16:56:40")),
                    new User("Eliza", "Bentley", DateTime.Parse("31/03/1996 19:41:40")),
                    new User("Steve", "Grenville", DateTime.Parse("19/08/1948 10:15:00")),
                    new User("Lainey", "Harmon", DateTime.Parse("17/04/1923 11:40:00")),
                    new User("Rosaline", "Kelsey", DateTime.Parse("25/10/1926 01:31:40")),
                    new User("Alvena", "Albinson", DateTime.Parse("26/01/1998 22:43:20")),
                    new User("Ivan", "Keighley", DateTime.Parse("03/06/1945 08:31:40")),
                    new User("Gordie", "Jack", DateTime.Parse("12/08/1930 22:46:40")),
                    new User("Delight", "Burnham", DateTime.Parse("15/11/1968 15:18:20")),
                    new User("Aleta", "Huddleson", DateTime.Parse("20/03/1976 02:18:20")),
                    new User("Hedley", "Lund", DateTime.Parse("25/12/1940 15:13:20")),
                    new User("Jeni", "Bristow", DateTime.Parse("18/01/1935 07:33:20")),
                    new User("Phyllis", "Waters", DateTime.Parse("17/03/1962 22:46:40")),
                    new User("Ryley", "Tuff", DateTime.Parse("30/03/1989 23:51:40")),
                    new User("Sharise", "Garrard", DateTime.Parse("18/06/1978 02:31:40")),
                    new User("Krysten", "Cannon", DateTime.Parse("04/02/1980 12:11:40")),
                    new User("Fulke", "Bullock", DateTime.Parse("02/03/2006 09:53:20")),
                    new User("Brand", "Chambers", DateTime.Parse("22/10/1992 18:51:40")),
                };
                context.Users.AddRange(users);
                context.SaveChanges();

                var roles = new List<Role>
                {
                    new Role { Description = "Admin", Name = "admin" },
                    new Role { Description = "Moderator", Name = "Moderator" },
                    new Role { Description = "Staff", Name = "Staff" },
                    new Role { Description = "Infomation Techonogies", Name = "IT" },
                    new Role { Description = "Finance", Name = "finance" },
                    new Role { Description = "Human Resources", Name = "HR" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();

                var userRoles = new List<UserRole>
                {
                    new UserRole(1, 4),  new UserRole(20, 1), new UserRole(12, 5),
                    new UserRole(17, 1), new UserRole(8, 3),  new UserRole(3, 3),
                    new UserRole(15, 1), new UserRole(12, 6), new UserRole(12, 4),
                    new UserRole(11, 1), new UserRole(4, 3),  new UserRole(16, 2),
                    new UserRole(10, 4), new UserRole(10, 6), new UserRole(14, 4),
                    new UserRole(2, 2),  new UserRole(18, 6), new UserRole(5, 5),
                    new UserRole(17, 6), new UserRole(9, 1),  new UserRole(9, 4)
                };
                context.UserRoles.AddRange(userRoles);
                context.SaveChanges();
            }
        }
    }
}
