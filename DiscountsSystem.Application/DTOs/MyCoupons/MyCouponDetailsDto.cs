using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.MyCoupons;

public sealed record MyCouponDetailsDto(
    int PurchaseId,
    int OfferId,
    string OfferTitle,
    string? OfferDescription,
    int Quantity,
    string CouponCode,
    CouponPurchaseStatus Status,
    DateTime PurchasedAtUtc,
    DateTime ExpiresAtUtc,
    int? ReservationId,
    PurchaseSourceType SourceType
);