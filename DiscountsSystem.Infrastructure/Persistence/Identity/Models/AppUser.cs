using Microsoft.AspNetCore.Identity;

namespace DiscountsSystem.Infrastructure.Persistence.Identity.Models;

public class AppUser : IdentityUser
{
    // Customer fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Merchant fields
    public string? CompanyName { get; set; }

    // Optional: audit
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}