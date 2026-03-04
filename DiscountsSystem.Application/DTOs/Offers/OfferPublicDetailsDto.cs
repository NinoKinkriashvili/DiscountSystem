using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Offers;

public record OfferPublicDetailsDto(
    int Id,
    string Title,
    string Description,
    decimal OriginalPrice,
    decimal DiscountPrice,
    int CouponQuantityTotal,
    int CouponQuantityAvailable,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    OfferStatus Status,
    int CategoryId,
    bool IsPurchaseBlockedByUpdatePending
);