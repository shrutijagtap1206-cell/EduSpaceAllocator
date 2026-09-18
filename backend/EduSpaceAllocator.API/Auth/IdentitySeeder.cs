using Microsoft.AspNetCore.Identity;

namespace EduSpaceAllocator.API.Auth;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles =
        {
            "Admin",
            "NGO",
            "EducationCoordinator",
            "Viewer"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }

        await CreateUser(
            userManager,
            "admin@eduspace.local",
            "Admin@12345",
            "Admin");

        await CreateUser(
            userManager,
            "ngo@eduspace.local",
            "NGO@12345",
            "NGO");

        await CreateUser(
            userManager,
            "coordinator@eduspace.local",
            "Coordinator@12345",
            "EducationCoordinator");

        await CreateUser(
            userManager,
            "viewer@eduspace.local",
            "Viewer@12345",
            "Viewer");
    }

    private static async Task CreateUser(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var existing =
            await userManager.FindByEmailAsync(email);

        if (existing != null)
        {
            if (!await userManager.IsInRoleAsync(existing, role))
            {
                await userManager.AddToRoleAsync(existing, role);
            }
            return;
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
