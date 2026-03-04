using DiscountsSystem.Application.DTOs.MerchantDashboard;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface IMerchantDashboardService
{
    Task<MerchantDashboardDto> GetMyDashboardAsync(CancellationToken ct = default);
}