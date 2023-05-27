using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
                    Email = "aranod42@email.com",
                    EmailConfirmed = true,
                };

                var result = await userMgr.CreateAsync(aranod, "Password0_");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRolesAsync(aranod, Roles);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddClaimsAsync(aranod, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Aranod"),
                        new Claim(JwtClaimTypes.GivenName, "Oleksii"),
                        new Claim(JwtClaimTypes.FamilyName, "Martynenko"),
                    });
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
