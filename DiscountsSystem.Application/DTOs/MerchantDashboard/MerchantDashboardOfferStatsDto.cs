namespace DiscountsSystem.Application.DTOs.MerchantDashboard;

public sealed record MerchantDashboardOfferStatsDto(
    int TotalOffersCount,
    int ActiveOffersCount,
    int ExpiredOffersCount,
    int PendingOffersCount,
    int ApprovedOffersCount,
    int RejectedOffersCount,
    int DisabledOffersCount,
    IReadOnlyList<MerchantOfferStatusCountDto> OfferStatusBreakdown
);