using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Offers;

public record OfferMerchantDetailsDto(
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
    bool IsActive,
    bool IsUpdatePending,
    int CategoryId,
    string MerchantId,
    string? RejectReason,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? ApprovedAtUtc,
    bool CanEditNow
);