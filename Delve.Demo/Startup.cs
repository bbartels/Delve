using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delve.Demo.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Delve.Demo.Models;
using Delve.Models.Validation;
using Delve.AspNetCore;

namespace Delve.Demo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddDelve();

            var connection = @"Server=(localdb)\mssqllocaldb;Database=DelveDemo;Trusted_Connection=True;";
            services.AddDbContext<UserManagerContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IQueryValidator<User>, UserQueryValidator>();


            var test = services.BuildServiceProvider().GetRequiredService<UserManagerContext>();
            test.Database.EnsureDeleted();
            test.Database.Migrate();
            test.EnsureSeeded();
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
                context.Users.Add(new Models.User()
                {
                    FirstName = "John",
                    LastName = "Smith",
                    DateOfBirth = DateTime.Now
                });

                context.SaveChanges();
            }
        }
    }
}
