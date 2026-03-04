using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.MerchantSales;

public sealed record MerchantSaleListItemDto(
    int PurchaseId,
    int OfferId,
    string OfferTitle,
    string CustomerId,
    int Quantity,
    string CouponCode,
    DateTime PurchasedAtUtc,
    DateTime ExpiresAtUtc,
    CouponPurchaseStatus Status,
    PurchaseSourceType SourceType,
    int? ReservationId
);