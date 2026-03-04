using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.MerchantDashboard;

public sealed record MerchantPurchaseStatusCountDto(
    CouponPurchaseStatus Status,
    int TotalCouponsCount
);