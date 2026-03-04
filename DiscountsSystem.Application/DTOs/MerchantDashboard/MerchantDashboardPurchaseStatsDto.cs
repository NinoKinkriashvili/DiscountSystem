namespace DiscountsSystem.Application.DTOs.MerchantDashboard;

public sealed record MerchantDashboardPurchaseStatsDto(
    int TotalPurchasesCount,
    int TotalSoldCouponsCount,
    int ActivePurchasedCouponsCount,
    int ExpiredPurchasedCouponsCount,
    IReadOnlyList<MerchantPurchaseStatusCountDto> PurchaseStatusBreakdown
);