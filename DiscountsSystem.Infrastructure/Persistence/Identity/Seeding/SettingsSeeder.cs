using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountsSystem.Infrastructure.Persistence.Seeding;

public static class SettingsSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var reservationMinutes = config.GetValue("SettingsDefaults:ReservationDurationMinutes", 30);
        var editHours = config.GetValue("SettingsDefaults:MerchantEditWindowHours", 24);

        var exists = await db.Settings.AnyAsync(x => x.Id == 1);
        if (exists) return;

        db.Settings.Add(new Settings
        {
            Id = 1,
            ReservationDurationMinutes = reservationMinutes,
            MerchantEditWindowHours = editHours
        });

        await db.SaveChangesAsync();
    }
}
