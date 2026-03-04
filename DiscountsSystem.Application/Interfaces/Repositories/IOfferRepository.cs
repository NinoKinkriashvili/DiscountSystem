using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Domain.Entities;

namespace DiscountsSystem.Application.Interfaces.Repositories;

public interface IOfferRepository
{
    Task<List<Offer>> GetPublicVisibleAsync(DateTime nowUtc, CancellationToken ct = default);
    Task<Offer?> GetPublicVisibleByIdAsync(int id, DateTime nowUtc, CancellationToken ct = default);

    Task<Offer?> GetByIdForUpdateAsync(int id, CancellationToken ct = default);

    Task<List<Offer>> GetByMerchantAsync(string merchantId, CancellationToken ct = default);

    Task<List<Offer>> GetPendingForModerationAsync(CancellationToken ct = default);
    Task<MerchantDashboardOfferStatsDto> GetMerchantDashboardOfferStatsAsync(
        string merchantId,
        DateTime nowUtc,
        CancellationToken ct = default);

    Task AddAsync(Offer offer, CancellationToken ct = default);
    Task UpdateAsync(Offer offer, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<int> ExpireDueAsync(DateTime nowUtc, CancellationToken ct = default);
}
