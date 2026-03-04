using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Application.DTOs.MerchantSales;
using DiscountsSystem.Application.DTOs.Purchases.Results;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task<(PurchaseResult Result, CouponPurchase? Purchase)> CreateDirectAsync(
        string customerId,
        int offerId,
        int quantity,
        DateTime nowUtc,
        CancellationToken ct = default);

    Task<(PurchaseResult Result, CouponPurchase? Purchase)> CreateFromReservationAsync(
        string customerId,
        int reservationId,
        DateTime nowUtc,
        CancellationToken ct = default);

    Task<int> ExpireDueCouponsAsync(DateTime nowUtc, CancellationToken ct = default);

    Task<CouponPurchase?> GetByIdAsync(int purchaseId, CancellationToken ct = default);

    Task<List<CouponPurchase>> GetByCustomerAsync(
        string customerId,
        CouponPurchaseStatus? status,
        CancellationToken ct = default);

    Task<List<MerchantSaleListItemDto>> GetMerchantSalesHistoryAsync(
        string merchantId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        int? offerId = null,
        CancellationToken ct = default);

    Task<MerchantDashboardPurchaseStatsDto> GetMerchantDashboardPurchaseStatsAsync(
        string merchantId,
        DateTime nowUtc,
        CancellationToken ct = default);
}
