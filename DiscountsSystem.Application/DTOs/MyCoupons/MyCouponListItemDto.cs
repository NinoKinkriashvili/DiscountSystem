using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.MyCoupons;

public sealed record MyCouponListItemDto(
    int PurchaseId,
    int OfferId,
    string OfferTitle,
    int Quantity,
    string CouponCode,
    CouponPurchaseStatus Status,
    DateTime PurchasedAtUtc,
    DateTime ExpiresAtUtc
);