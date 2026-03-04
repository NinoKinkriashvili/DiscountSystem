using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Services.Merchant;

public sealed class MerchantDashboardService : IMerchantDashboardService
{
    private readonly IOfferRepository _offers;
    private readonly IPurchaseRepository _purchases;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _time;

    public MerchantDashboardService(
        IOfferRepository offers,
        IPurchaseRepository purchases,
        ICurrentUserService currentUser,
        IDateTimeProvider time)
    {
        _offers = offers;
        _purchases = purchases;
        _currentUser = currentUser;
        _time = time;
    }

    public async Task<MerchantDashboardDto> GetMyDashboardAsync(CancellationToken ct = default)
    {
        EnsureMerchant();

        var merchantId = _currentUser.UserId!;
        var nowUtc = _time.UtcNow;

        var offerStats = await _offers.GetMerchantDashboardOfferStatsAsync(merchantId, nowUtc, ct);
        var purchaseStats = await _purchases.GetMerchantDashboardPurchaseStatsAsync(merchantId, nowUtc, ct);

        return new MerchantDashboardDto(
            // Offer statistics
            TotalOffers: offerStats.TotalOffersCount,
            ActiveOffersCount: offerStats.ActiveOffersCount,
            ExpiredOffersCount: offerStats.ExpiredOffersCount,
            PendingOffersCount: offerStats.PendingOffersCount,
            ApprovedOffersCount: offerStats.ApprovedOffersCount,
            RejectedOffersCount: offerStats.RejectedOffersCount,
            DisabledOffersCount: offerStats.DisabledOffersCount,

            // Purchase statistics
            TotalPurchasesCount: purchaseStats.TotalPurchasesCount,
            TotalSoldCouponsCount: purchaseStats.TotalSoldCouponsCount,
            ActivePurchasedCouponsCount: purchaseStats.ActivePurchasedCouponsCount,
            ExpiredPurchasedCouponsCount: purchaseStats.ExpiredPurchasedCouponsCount,

            // Breakdowns
            OfferStatusBreakdown: offerStats.OfferStatusBreakdown,
            PurchaseStatusBreakdown: purchaseStats.PurchaseStatusBreakdown,

            // Snapshot time
            GeneratedAtUtc: nowUtc
        );
    }

    private void EnsureMerchant()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (_currentUser.Role is not UserRole.Merchant)
            throw new UnauthorizedAccessException("Only Merchant can perform this action.");
    }
}
