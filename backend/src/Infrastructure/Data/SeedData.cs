using Core.Entities;
using Core.Enums;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEncryptionService encryptionService,
        IConfiguration configuration
    )
    {
        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Admin User
        await SeedAdminUserAsync(userManager, encryptionService, configuration);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            ApplicationRoles.Admin,
            ApplicationRoles.Presenter,
            ApplicationRoles.Participant
        };

        foreach (var role in roles)
        {
            var roleName = role.ToValueString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var identityRole = new IdentityRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                await roleManager.CreateAsync(identityRole);
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IEncryptionService encryptionService,
        IConfiguration configuration
    )
    {
        var adminConfig = configuration.GetSection("AdminUser");
        var adminEmail = adminConfig["Email"] ?? "admin@dhamma.org";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Get encrypted password from configuration and decrypt it
            var encryptedPassword = adminConfig["EncryptedPassword"];

            if (string.IsNullOrEmpty(encryptedPassword))
            {
                throw new InvalidOperationException(
                    "Admin encrypted password not found in configuration"
                );
            }

            // Decrypt the password using EncryptionService
            var decryptedPassword = encryptionService.Decrypt(encryptedPassword);

            var result = await userManager.CreateAsync(user, decryptedPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, ApplicationRoles.Admin.ToValueString());

                // TODO: Create UserProfile with FirstName="Admin", LastName="User" via event
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }
        }
    }
}
