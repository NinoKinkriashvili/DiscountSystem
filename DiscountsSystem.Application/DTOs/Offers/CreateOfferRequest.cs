namespace DiscountsSystem.Application.DTOs.Offers;

public record CreateOfferRequest(
    string Title,
    string Description,
    decimal OriginalPrice,
    decimal DiscountPrice,
    int CouponQuantityTotal,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    int CategoryId
);