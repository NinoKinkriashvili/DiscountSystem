namespace DiscountsSystem.Application.DTOs.Offers;

public record UpdateOfferRequest(
    string Title,
    string Description,
    decimal OriginalPrice,
    decimal DiscountPrice,
    int CouponQuantityTotal,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    int CategoryId
);