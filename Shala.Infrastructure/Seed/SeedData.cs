using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shala.Domain.Entities.Identity;

namespace Shala.Infrastructure.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles =
        {
            "SuperAdmin",
            "SchoolAdmin",
            "BranchAdmin",
            "Teacher",
            "Accountant",
            "Staff"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string superAdminEmail = "owner@shala.com";

        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin is null)
        {
            var user = new ApplicationUser
            {
                FullName = "Platform Owner",
                UserName = superAdminEmail,
                Email = superAdminEmail,
                TenantId = null,
                BranchId = null,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Owner@12345");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}