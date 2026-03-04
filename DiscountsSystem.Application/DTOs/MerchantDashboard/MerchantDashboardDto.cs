namespace DiscountsSystem.Application.DTOs.MerchantDashboard;

public sealed record MerchantDashboardDto(
    // Offer statistics
    int TotalOffers,
    int ActiveOffersCount,
    int ExpiredOffersCount,
    int PendingOffersCount,
    int ApprovedOffersCount,
    int RejectedOffersCount,
    int DisabledOffersCount,

    // Sales / purchases statistics
    int TotalPurchasesCount,
    int TotalSoldCouponsCount,
    int ActivePurchasedCouponsCount,
    int ExpiredPurchasedCouponsCount,

    // Optional breakdowns
    IReadOnlyList<MerchantOfferStatusCountDto> OfferStatusBreakdown,
    IReadOnlyList<MerchantPurchaseStatusCountDto> PurchaseStatusBreakdown,

    // Snapshot time
    DateTime GeneratedAtUtc
);
