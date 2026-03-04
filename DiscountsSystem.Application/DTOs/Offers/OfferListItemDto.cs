using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Offers;

public record OfferListItemDto(
    int Id,
    string Title,
    decimal DiscountPrice,
    DateTime EndDateUtc,
    int CouponQuantityAvailable,
    OfferStatus Status,
    int CategoryId
);