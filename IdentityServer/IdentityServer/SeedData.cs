using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer
{
    public static class SeedData
    {
        public static string[] Roles = new string[]
        {
            "security",
            "access-control",
            "customer-management",
            "contract-management",
            "customer-support",

            "data",
            "data-configuration",
            "cc-mapping",
            "gl-pr-mapping",
            "data-entry",
            "data-administration",

            "reporting",
            "management",
            "item-sharing",
            "administration",
             "generic-reports",
             "department-reports"
        };

        public static async void EnsureSeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            if (!context.Roles.Any())
            {
                foreach (var role in Roles)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            if (!context.Users.Any())
            {
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var aranod = new User
                {
                    UserName = "aranod",
                    CustomerId = 6,
                    CustomerName = "Internal Customer"
                };

                var result = await userMgr.CreateAsync(aranod, "_Password0_");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRolesAsync(aranod, Roles);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
