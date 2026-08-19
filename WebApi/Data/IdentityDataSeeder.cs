using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            "Admin",
            "User"
        };

        foreach (var roleName in roles)
        {
            var roleExists =
                await roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await roleManager.CreateAsync(
                    new IdentityRole(roleName));
            }
        }
    }

    public static async Task SeedAdminAsync(UserManager<User> userManager)
    {
        const string adminUserName = "Mustafa33";

        var adminUser = await userManager.FindByNameAsync(adminUserName);

        if (adminUser is null)
        {
            return;
        }

        var isAdmin = await userManager.IsInRoleAsync(adminUser, "Admin");

        if (!isAdmin)
        {
            await userManager.AddToRoleAsync(adminUser,"Admin");
        }
    }
}