using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Application.Services.Categories;
using DiscountsSystem.Application.Services.Merchant;
using DiscountsSystem.Application.Services.Offers;
using DiscountsSystem.Application.Services.Purchases;
using DiscountsSystem.Application.Services.Reservations;
using DiscountsSystem.Application.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountsSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IMerchantSalesHistoryService, MerchantSalesHistoryService>();
        services.AddScoped<IMerchantDashboardService, MerchantDashboardService>();

        return services;
    }
}