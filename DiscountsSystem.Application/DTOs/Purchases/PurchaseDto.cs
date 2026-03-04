using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Purchases;

public sealed record PurchaseDto(
    int Id,
    int OfferId,
    int Quantity,
    int? ReservationId,
    string CouponCode,
    PurchaseSourceType SourceType,
    CouponPurchaseStatus Status,
    DateTime ExpiresAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);