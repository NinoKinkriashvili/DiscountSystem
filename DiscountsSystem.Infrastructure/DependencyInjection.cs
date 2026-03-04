using DiscountsSystem.Application.Interfaces.Auth;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Infrastructure.Persistence.Context;
using DiscountsSystem.Infrastructure.Persistence.Identity.Models;
using DiscountsSystem.Infrastructure.Repositories;
using DiscountsSystem.Infrastructure.Services.Auth;
using DiscountsSystem.Infrastructure.Services.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountsSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DiscountsDb")
                 ?? throw new InvalidOperationException("Connection string 'DiscountsDb' not found.");

        services.AddDbContext<DiscountsDbContext>(opt =>
            opt.UseSqlServer(cs, sql =>
                sql.MigrationsAssembly(typeof(DiscountsDbContext).Assembly.GetName().Name)));

        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<DiscountsDbContext>()
            .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();

        // Auth
        services.AddScoped<IJwtHelper, JwtHelper>();
        services.AddScoped<IAuthService, AuthService>();

        // Common
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}