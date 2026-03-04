using DiscountsSystem.Domain.Enums;
using DiscountsSystem.Infrastructure.Persistence.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountsSystem.Infrastructure.Persistence.Identity.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        // 1) Roles
        var roles = new[] { UserRole.Administrator, UserRole.Merchant, UserRole.Customer };

        foreach (var role in roles)
        {
            var roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // 2) Seed Admin (from config)
        var enabled = config.GetValue<bool>("Seed:Admin:Enabled");
        if (!enabled) return;

        var adminEmail = config["Seed:Admin:Email"];
        var adminPassword = config["Seed:Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException("Seed admin is enabled but Email/Password is missing in configuration.");

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var createRes = await userManager.CreateAsync(admin, adminPassword);
            if (!createRes.Succeeded)
            {
                var msg = string.Join("; ", createRes.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Seed admin create failed: {msg}");
            }
        }

        // 3) Ensure role
        var isInRole = await userManager.IsInRoleAsync(admin, UserRole.Administrator.ToString());
        if (!isInRole)
        {
            var addRoleRes = await userManager.AddToRoleAsync(admin, UserRole.Administrator.ToString());
            if (!addRoleRes.Succeeded)
            {
                var msg = string.Join("; ", addRoleRes.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Seed admin add role failed: {msg}");
            }
        }
    }
}