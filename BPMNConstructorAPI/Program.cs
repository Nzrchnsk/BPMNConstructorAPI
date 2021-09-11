using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPMNConstructorAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BPMNConstructorAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            if (!rolesManager.Roles.Any())
            {
                await rolesManager.CreateAsync(new IdentityRole<int>("Admin"));
                await rolesManager.CreateAsync(new IdentityRole<int>("User"));
            }
            
            if (!userManager.Users.Any())
            {
                var admin = new User
                {
                    Email = "admin@example.com",
                    UserName = "admin",
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(admin, "12345_Aa");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}